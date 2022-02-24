import pygame
import sys
from perlin_noise import PerlinNoise


SAND = "#117c13"
WALL = "#000000"
WATER = "#d4f1f9"
MUD = "#70543e"

count = {"sand":0,"wall":0,"water":0,"mud":0}
limits = [0,0,0,0]

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


def SetLimits(terrain,obstacles,totalCount,ratio,obsRatio):
    """
    Funcao recebe o terreno em que ela vai agir e seta os limites para que a porcentagem dos elementos
    definidas em ratio seja respeitada ratio = [%water,%mud,%sand] (somando tem que ser igual a 1), 
    obsRatio = %wall gerando um mundo com essas caracteristicas
    """


    localTerrain = [value for row in terrain for value in row]
    localObstacle = [value for row in obstacles for value in row]

    localObstacle.sort()
    localTerrain.sort()

    localCount = 0
    element = 0
    for value in localTerrain:
        localCount += 1
        if localCount/totalCount >=ratio[element]:
            limits[element] = value
            element+=1
            localCount=0

    
    localCount = 0 
    element+=1
    for value in localObstacle:
        localCount += 1
        if localCount/totalCount >= obsRatio:
            limits[element] = value
            break 
    
def GenerateTerrain(map,obstacles,dimX,dimY):

    terrain = [["" for _ in range(dimX)] for _ in range(dimY)]

    for rowIndex,row in enumerate(map):
        for colIndex,value in enumerate(row):
            x,y = colIndex,rowIndex

            if(value < limits[0]):
                terrain[y][x] = "water"
            elif(value < limits[1]):
                terrain[y][x] = "mud"
            else:
                terrain[y][x] = "sand"

    for rowIndex,row in enumerate(obstacles):
        for colIndex,value in enumerate(row):
            x,y = colIndex,rowIndex
            if(value < limits[3]):
                terrain[y][x] = "wall"
    
    return terrain

def GetTerrain(dimX,dimY):

    """
    Gera uma matrix dimX * dimY com os valores "water" "mud" "sand" e "wall"
    """


    #generation random terrain with Perlin noise
    nOctaves = 5
    noiseGenerator1 = PerlinNoise(octaves = nOctaves)
    noiseGenerator2 = PerlinNoise(octaves=nOctaves*3)

    map = [[noiseGenerator1([x*(1/dimX),y*(1/dimX)]) for x in range(dimX)] for y in range(dimY)]
    obstacles = [[noiseGenerator2([x*(1/dimX),y*(1/dimX)]) for x in range(dimX)] for y in range(dimY)]

    SetLimits(map,obstacles,dimX*dimY,[1/3,1/3,1/3],2/10)
    terrain = GenerateTerrain(map,obstacles,dimX,dimY)

    return terrain

if __name__=="__main__":
    dimX, dimY = 50,50
    blockSize = 10

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