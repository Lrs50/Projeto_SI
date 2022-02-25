from collections import deque

class Queue(object):

    def __init__(self,*args):
        self._vector = deque()
        for i in args:
            self._vector.append(i)
    def Enqueue(self,item,*args):
        self._vector.append(item)
        for i in args:
            self._vector.append(i)

    def Dequeue(self):
        return self._vector.popleft() 

    def Front(self):
        return self._vector[0]

    def Rear(self):
        return self._vector[-1]

    def __str__(self) -> str:
        out = "Queue("
        for i in self._vector:
            out+=str(i)+","
        out = out[:-1] + ")"
        return out

    def Empty(self):
        return len(self._vector)==0


class Stack(object):
    def __init__(self,*args):
        self._vector = deque()
        for i in args:
            self._vector.append(i)
    def Push(self,item,*args):
        self._vector.append(item)
        for i in args:
            self._vector.append(i)

    def Pop(self):
        return self._vector.pop() 

    def Top(self):
        return self._vector[-1]

    def __str__(self) -> str:
        out = "Stack("
        for i in self._vector:
            out+=str(i)+","
        out = out[:-1] + ")"
        return out

    def Empty(self):
        return len(self._vector)==0

if __name__=="__main__":
    queue = Queue(*[2,3,4,5,6,7,8])
    print(queue)
    print(queue.Dequeue())
    print(queue)
    queue.Enqueue(10,11,12)
    print(queue)