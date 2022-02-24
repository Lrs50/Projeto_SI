import pygame
import sys
from perlin_noise import PerlinNoise


SAND = "#117c13"
WALL = "#000000"
WATER = "#d4f1f9"
MUD = "#70543e"

count = {"sand":0,"wall":0,"water":0,"mud":0}
limits = [0,0,0]

def DrawTerrain(surfuce,reference,blockSize):

    """
    Usa a informacao que esta na matriz terreno e os limites setados pelo usuario para gerar
    uma representacao visual do mapa.
    """

    offset = blockSize//2
    for rowIndex,row in enumerate(reference):
        for colIndex,value in enumerate(row):
            x = rowIndex * blockSize +offset
            y = colIndex * blockSize +offset

            color = ""

            if(value < limits[0]):
                color = WATER
                count["water"]+=1 
            elif(value < limits[1]):
                color = MUD
                count["mud"]+=1
            elif(value < limits[2]):
                color = SAND
                count["sand"]+=1
            else:
                color = WALL
                count["wall"]+=1

            image = pygame.Surface((blockSize,blockSize))
            image.fill(color)
            rect = image.get_rect(topleft = (x,y))
            surfuce.blit(image,rect)


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


if __name__=="__main__":
    dimX, dimY = 50,50
    blockSize = 10


    #generation random terrain with Perlin noise

    noiseGenerator = PerlinNoise(octaves = 5)

    terreain = [[abs(noiseGenerator([x*(1/dimX),y*(1/dimX)])) for x in range(dimX)] for y in range(dimY)]

    SetLimits(terreain,dimX*dimY,[0.2,0.2,0.3,0.3])

    #pygame stuff

    width = (dimX+1)*(blockSize)
    height = (dimY+1)*(blockSize)

    pygame.init()
    win = pygame.display.set_mode((width,height))
    clock= pygame.time.Clock()

    DrawTerrain(win,terreain,blockSize)
    print(count)

    while(True):

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()

        win.fill((30,30,30))
        DrawTerrain(win,terreain,blockSize)
        pygame.display.flip()
        clock.tick(60)

    pass