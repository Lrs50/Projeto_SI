import pygame
import sys
from perlin_noise import PerlinNoise


SAND = "#117c13"
WALL = "#000000"
WATER = "#d4f1f9"
MUD = "#70543e"

count = {"water":0,"mud":0,"sand":0,"wall":0}
limits = [0,0,0]

def DrawTerrain(surface,reference,blockSize):

    """
    Usa a informacao que esta na matriz terreno e os limites setados pelo usuario para gerar
    uma representacao visual do mapa.
    """

    offset = blockSize//2
    for rowIndex,row in enumerate(reference):
        for colIndex,value in enumerate(row):
            x = colIndex * blockSize +offset
            y = rowIndex * blockSize +offset

            color = "#ff0000"

            if(value == "water" ):
                color = WATER
               
            elif(value == "mud" ):
                color = MUD
                
            elif(value == "sand" ):
                color = SAND
               
            elif(value == "wall"):
                color = WALL

                
            count[value]+=1

            image = pygame.Surface((blockSize,blockSize))
            image.fill(color)
            rect = image.get_rect(topleft = (x,y))
            surface.blit(image,rect)

def GenerateTerrain(map,dimX,dimY):

    terrain = [["" for _ in range(dimX)] for _ in range(dimY)]

    for rowIndex,row in enumerate(map):
        for colIndex,value in enumerate(row):
            x,y = colIndex,rowIndex

            if(value < limits[0]):
                terrain[y][x] = "water"
            elif(value < limits[1]):
                terrain[y][x] = "mud"
            elif(value <limits[2]):
                terrain[y][x] = "sand"
            else:
                terrain[y][x] = "wall"
    
    return terrain

def SetLimits(terrain,totalCount,ratio):
    """
    Funcao recebe o terreno em que ela vai agir e seta os limites para que a porcentagem dos elementos
    definidas em ratio seja respeitada ratio = [%water,%mud,%sand,%wall] gerando um mundo com essas
    caracteristicas
    """


    localTerrain = [value for row in terrain for value in row]
    localTerrain.sort()
    localCount = 0
    element = 0
    for value in localTerrain:
        localCount += 1
        if localCount/totalCount >=ratio[element]:
            limits[element] = value
            element+=1
            localCount=0
        if element == 3:
            break

def GetTerrain(dimX,dimY):

    """
    Gera uma matrix dimX * dimY com os valores "water" "mud" "sand" e "wall"
    """

    #generation random terrain with Perlin noise

    noiseGenerator = PerlinNoise(octaves = 10)

    terrain = [[noiseGenerator([x*(1/dimX),y*(1/dimX)]) for x in range(dimX)] for y in range(dimY)]

    SetLimits(terrain,dimX*dimY,[0.2,0.3,0.3,0.2])
    terrain = GenerateTerrain(terrain,dimX,dimY)

    return terrain

if __name__=="__main__":
    dimX, dimY = 100,100
    blockSize = 7

    terrain = GetTerrain(dimX,dimY)

    #pygame stuff

    width = (dimX+1)*(blockSize)
    height = (dimY+1)*(blockSize)

    pygame.init()
    win = pygame.display.set_mode((width,height))
    clock= pygame.time.Clock()

    DrawTerrain(win,terrain,blockSize)
    print(count)

    while(True):

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()

        win.fill((30,30,30))
        DrawTerrain(win,terrain,blockSize)
        pygame.display.flip()
        clock.tick(60)

    pass