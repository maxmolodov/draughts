# draughts
Draughts (Checkers)

Task:
Написать WPF приложение, которое отображает шашечную доску размером 8 x 8 с шашками на ней. 

Обеспечить выполнение следующих функций:

1. Произвольная расстановка белых и черных шашек.
2. Изменение размеров доски вместе с изменением размеров формы.
3. Если задана позиция, в которой белыми шашками должен быть выполнен вынужденный ход (взятие черных шашек), сделать этот ход по нажатию кнопки. Ход делать с учетом того, что шашка, побывавшая на последней горизонтали, становится дамкой.

Правила хода:
1. Белые ходят снизу вверх.
2. Бить обязательно. Если есть несколько вариантов побития, выбирается один из тех, где бьется максимальное количество шашек противника.
3. Белая шашка, побывавшая на самой верхней горизонтали, становится дамкой. Если она попадает на верхнюю горизонталь во время своего хода, то продолжает ход уже в качестве дамки.
4. Дамка может бить, возвращаясь по уже пройденной диагонали.

Deadline: 3 days
Note: First experience with WPF.
