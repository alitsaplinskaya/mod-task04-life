# МИПиС

## Task 04. Исследование клеточных автоматов на примере игры "Жизнь"

Наиболее широко известным клеточным автоматом является ”Игра Жизнь” (Джон Конвей, 1970).

Правила:

- Каждая клетка имеет 2 состояния: жива,мертва.
- Каждая клетка имеет 8 соседей.
- Распределение живых клеток на решетке называется поколением.
- В мёртвой клетке, рядом с которой ровно три живые клетки, зарождается жизнь.
- Если у живой клетки есть две или три живые соседки, то эта клетка продолжает жить; в противном случае, если соседей меньше двух или больше трёх, клетка умирает (”от одиночества” или ”от перенаселённости”).

Реализация будет включать в себя классы:

- **Cell** - представление ячейки.
- **Board** - представление всей решетки.
- **Program** - класс консольного приложения.

В ходе полевых испытаний этой игры, сначала на бумаге, а затем и с помощью компьютерного моделирования было выявлено множество фигур, которые по их поведению можно рассортировать по нескольким классам.

- Устойчивые фигуры: фигуры, которые остаются неизменными
- Периодические фигуры: фигуры, у которых состояние повторяется
через некоторое число поколений
- Двигающиеся фигуры: фигуры, у которых состояние повторяется, но с некоторым смещением
- Ружья: фигуры, у которых состояние повторяется, но дополнительно появляется двигающаяся фигура
- Паровозы: двигающиеся фигуры, которые оставляют за собой следы в виде устойчивых или периодических фигур
- Пожиратели: устойчивые фигуры, которые могут пережить столкновения с некоторыми двигающимися фигурами

### Задача №1 (Доработка консольного приложения)

- Создать файл с настройками, позволяющими менять параметры КЛА (json-формат).
- Разработать возможность сохранения состояния системы в текстовый файл, загрузку состояния системы из файла и продолжение моделирования.
- Подготовить набор файлов с заранее определенными фигурами-колониями, провести загрузку и изучить процесс моделирования.

### Задача №2 (Исследование КЛА)

- Подсчитать количество элементов (клеток+комбинаций) на поле
- Классифицировать элементы, сопоставляя с образцами
- Исследовать среднее время (число поколений) перехода в стабильную фазу
- Подсчитать количество симметричных элементов, исследование симметричности всей системы от числа поколений


### Структура проекта

Что дано:

- **Life/Program.cs** - файл с реализацией классов

Проект можно расширять для решения поставленных задач
 
### Список участников/веток

см. репозиторий `mod-branches`

### Алгоритм выполнения работы

Для выполнения работы необходимо:

1. Выполнить *fork* репозитария в свой аккаунт.
1. Выполнить клонирование репозитария из своего аккаунта к себе на локальную машину (`git clone`).
1. Создать ветку **git** с индивидуальным номером (`git branch имя_ветки`).
1. Сделать ветку активной (`git checkout имя`).
1. Необходимо разместить как исходные файлы с решениями задач, поместив **cpp** файлы в **src**, а заголовочные - в **include**. 
1. Добавить файлы в хранилище (`git add`).
1. Выполнить фиксацию изменений (`git commit -m "комментарий"`).
1. Отправить содержимое ветки в свой удаленный репозитарий (`git push origin имя_ветки`).
1. Создать пул-запрос в репозитарий группы и ждать результата от **GitHub Actions**.
