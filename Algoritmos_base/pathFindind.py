from numpy import size
import pygame
from sqlalchemy import null
from randomTerrain import GetTerrain
import sys
from DataStructures import *
from pprint import pprint
from random import randrange

SAND = "#117c13"
WALL = "#000000"
WATER = "#d4f1f9"
MUD = "#70543e"

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

            elif(value == "food"):
                color = "red"

            elif(value == "start"):
                color="white"
            

            image = pygame.Surface((blockSize,blockSize))
            image.fill(color)
            rect = image.get_rect(topleft = (x,y))
            surface.blit(image,rect)


def Neighbor(terrain,pos):
    y,x = pos[0],pos[1]

    sizeY = len(terrain)
    sizeX = len(terrain[0])

    neighbors= []

    if(x-1>=0):
        neighbors.append((y,x-1))
    if(y+1<sizeY):
        neighbors.append((y+1,x))
    if(x+1<sizeX):
        neighbors.append((y,x+1))
    if(y-1>=0):
        neighbors.append((y-1,x))

    neighbors = list(filter(lambda item: terrain[item[0]][item[1]]!="wall",neighbors))
    return neighbors


def BreathFirstSearch(terrain,start,end):
    states = {}

    #possible status: undiscovered, discovered, visited

    for rowIndex,row in enumerate(terrain):
        for colIndex,item in enumerate(row):
            if item == "wall":
                continue
            states[f"{(rowIndex,colIndex)}"] = {"status":"undiscovered","distance":null,"previous":null}

    done = False
    current = start
    states[str(current)]["status"] = "discovered"
    states[str(current)]["distance"] = 0
    queue = Queue()
    while(not done):
        neighbors = Neighbor(terrain,current)
        done = True

        for node in neighbors:
            if states[str(node)]["status"]=="undiscovered":
                done = False
                states[str(node)]["status"]="discovered"
                states[str(node)]["distance"]=states[str(current)]["distance"]+1
                states[str(node)]["previous"] = current
                queue.Enqueue(node)

        states[str(current)]["status"] = "visited"

        if done and queue.Empty():
            continue

        done = False
        current = queue.Dequeue()

    #translates the states dict to a list of positions to the food 
    current = end
    path = deque()
    while(current!=start):
        if current == null:
            path = deque()
            break
        path.appendleft(current)
        current = states[str(current)]["previous"]

    path.appendleft(start)
    return path


def DrawPath(surface,path,blockSize):

    coords = [ ((x+1)*blockSize ,(y+1)*blockSize) for y,x in path]

    pygame.draw.lines(surface,"red",False,coords)

def RandomPos(terrain,dimX,dimY):
    x,y = randrange(dimX),randrange(dimY)
    while(terrain[y][x]=="wall" or len(Neighbor(terrain,(y,x)))<1):
        x,y = randrange(dimX),randrange(dimY)

    return (y,x)

if __name__=="__main__":

    dimX, dimY = 100,100
    blockSize = 7

    terrain = GetTerrain(dimX,dimY)

    foodPos= (99,99)#RandomPos(terrain,dimX,dimY)
    startPos= (0,0)  #RandomPos(terrain,dimX,dimY)

    terrain[startPos[0]][startPos[1]] = "start"
    terrain[foodPos[0]][foodPos[1]] = "food"

    path = BreathFirstSearch(terrain,startPos,foodPos)

    #pygame stuff

    width = (dimX+1)*(blockSize)
    height = (dimY+1)*(blockSize)

    pygame.init()
    win = pygame.display.set_mode((width,height))
    clock= pygame.time.Clock()

    DrawTerrain(win,terrain,blockSize)

    while(True):

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()

        win.fill((30,30,30))
        DrawTerrain(win,terrain,blockSize)
        if len(path)>1:
            DrawPath(win,path,blockSize)
        pygame.display.flip()
        clock.tick(60)
