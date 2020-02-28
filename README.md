# QSimulator

Quiz rule simulator for Windows

Developed by CSharp

# Usage 

## Code example

Write rules in code area.

For example(7o3x):

```
<Init>
count:5

<Ident>
o
x=0

<Win>
o>=7

<Lose>
x>=3

<Correct>
o++

<Wrong>
x++
```

## Terminal example

Write players' correct/wrong in terminal line(bottom textbox).

For example:

Player1 Correct -> Player2 Wrong -> Through -> Player4 Wrong -> Player1 Correct


```
1o2xt4x1o
```

# LICENSE
[MIT](LICENSE)
