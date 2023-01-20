//случайно выпадающие бонусы с доп баллами(1к, 5к, 10к). Появляются независимо от яблок на 10 сек.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Поле на котором живет змея
        Entity field;
        // голова змеи
        Head head;
        // вся змея
        List<PositionedEntity> snake;
        // яблоко
        Apple apple;
        // банан
        Banan banan;
        // Огурец
        Cucumber cucumber;
        // Огурчик-Рик
        Cucumber_Rick cucumber_rick;
        //количество очков
        int score;
        //таймер по которому 
        DispatcherTimer moveTimer;
        //таймер по которому 
        DispatcherTimer moveTimer2;

        //конструктор формы, выполняется при запуске программы
        public MainWindow()
        {
            InitializeComponent();

            snake = new List<PositionedEntity>();
            //создаем поле 300х300 пикселей
            field = new Entity(600, 600, "pack://application:,,,/Resources/snake.png");

            //создаем таймер срабатывающий раз в 10 с
            moveTimer2 = new DispatcherTimer();
            moveTimer2.Interval = new TimeSpan(0, 0, 0, 0, 10000);
            moveTimer2.Tick += new EventHandler(moveTimer_Tick2);

            //создаем таймер срабатывающий раз в 300 мс
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
        }

        //метод перерисовывающий экран
        private void UpdateField()
        {
            //обновляем положение элементов змеи
            foreach (var p in snake)
            {
                Canvas.SetTop(p.image, p.y);
                Canvas.SetLeft(p.image, p.x);
            }

            //обновляем положение яблока
            Canvas.SetTop(apple.image, apple.y);
            Canvas.SetLeft(apple.image, apple.x);
            /*
            //обновляем положение банана
            Canvas.SetTop(banan.image, banan.y);
            Canvas.SetLeft(banan.image, banan.x);

            //обновляем положение огурца
            Canvas.SetTop(cucumber.image, cucumber.y);
            Canvas.SetLeft(cucumber.image, cucumber.x);

            //обновляем положение огурчика-Рика
            Canvas.SetTop(cucumber_rick.image, cucumber_rick.y);
            Canvas.SetLeft(cucumber_rick.image, cucumber_rick.x);
            */
            //обновляем количество очков
            lblScore.Content = String.Format("{0}000", score);
        }

        int key = 1;

        void moveTimer_Tick2(object sender, EventArgs e)
        {
            switch (key)
            {
                case 0:
                    key = 1;
                    break;
                case 1: // создаем новый банан и добавлем его
                    banan = new Banan(snake);
                    canvas1.Children.Add(banan.image);
                    //обновляем положение банана
                    Canvas.SetTop(banan.image, banan.y);
                    Canvas.SetLeft(banan.image, banan.x);
                    key = 2;
                    break;
                case 2: //Удаляем банан
                    banan.x = 0;
                    banan.y = 0;
                    canvas1.Children.Remove(banan.image);                   
                    key = 3;
                    break;
                case 3:// создаем новый огурец и добавлем его
                    cucumber = new Cucumber(snake);
                    canvas1.Children.Add(cucumber.image);
                    //обновляем положение огурца
                    Canvas.SetTop(cucumber.image, cucumber.y);
                    Canvas.SetLeft(cucumber.image, cucumber.x);
                    key = 4;
                    break;
                case 4: //Удаляем огурчик
                    cucumber.x = 0;
                    cucumber.y = 0;
                    canvas1.Children.Remove(cucumber.image);
                    key = 5;
                    break;
                case 5: // создаем нового огурчика-Рика и добавлем его
                    cucumber_rick = new Cucumber_Rick(snake);
                    canvas1.Children.Add(cucumber_rick.image);
                    //обновляем положение огурчика-Рика
                    Canvas.SetTop(cucumber_rick.image, cucumber_rick.y);
                    Canvas.SetLeft(cucumber_rick.image, cucumber_rick.x);
                    key = 6;
                    break;
                case 6: //Удаляем огурчика-Рика
                    cucumber_rick.x = 0;
                    cucumber_rick.y = 0;
                    canvas1.Children.Remove(cucumber_rick.image);
                    key = 1;
                    break;
            }
        }

        //обработчик тика таймера. Все движение происходит здесь
        void moveTimer_Tick(object sender, EventArgs e)
        {
            //в обратном порядке двигаем все элементы змеи
            foreach (var p in Enumerable.Reverse(snake))
            {
                p.move();
            }

            //проверяем, что голова змеи не врезалась в тело
            foreach (var p in snake.Where(x => x != head))
            {
                //если координаты головы и какой либо из частей тела совпадают
                if (p.x == head.x && p.y == head.y)
                {
                    //мы проиграли
                    moveTimer.Stop();
                    moveTimer2.Stop();
                    tbGameOver.Visibility = Visibility.Visible;
                    return;
                }
            }

            //проверяем, что голова змеи не вышла за пределы поля
            if (head.x < 40 || head.x >= 540 || head.y < 40 || head.y >= 540)
            {
                //мы проиграли
                moveTimer.Stop();
                moveTimer2.Stop();
                tbGameOver.Visibility = Visibility.Visible;
                return;
            }

            //проверяем, что голова змеи врезалась в яблоко
            if (head.x == apple.x && head.y == apple.y)
            {
                //увеличиваем счет
                score++;
                //двигаем яблоко на новое место
                apple.move();
                // добавляем новый сегмент к змее
                var part = new BodyPart(snake.Last());
                canvas1.Children.Add(part.image);
                snake.Add(part);
            }


            //проверяем, что голова змеи врезалась в банан
            if (banan != null)
                if (head.x == banan.x && head.y == banan.y)
                {
                    //увеличиваем счет
                    score++;
                    //двигаем банан на новое место
                    //banan.move();
                    // добавляем новый сегмент к змее
                    var part = new BodyPart(snake.Last());
                    canvas1.Children.Add(part.image);
                    snake.Add(part);
                    //Удаляем банан
                    banan.x = 0;
                    banan.y = 0;
                    canvas1.Children.Remove(banan.image);                   
                    key = 3;
                }

            //проверяем, что голова змеи врезалась в огурец
            if (cucumber != null)
                if (head.x == cucumber.x && head.y == cucumber.y)
                {
                    //увеличиваем счет
                    score = score + 5;
                    //двигаем огурец на новое место
                    cucumber.move();
                    // добавляем новый сегмент к змее
                    var part = new BodyPart(snake.Last());
                    canvas1.Children.Add(part.image);
                    snake.Add(part);
                    //Удаляем огурчик
                    cucumber.x = 0;
                    cucumber.y = 0;
                    canvas1.Children.Remove(cucumber.image);
                    key = 5;
                }

            //проверяем, что голова змеи врезалась в огурчика-Рика
            if (cucumber_rick != null)
                if (head.x == cucumber_rick.x && head.y == cucumber_rick.y)
                {
                    //увеличиваем счет
                    score = score + 10;
                    //двигаем огурчика-Рика на новое место
                    cucumber_rick.move();
                    // добавляем новый сегмент к змее
                    var part = new BodyPart(snake.Last());
                    canvas1.Children.Add(part.image);
                    snake.Add(part);
                    key = 1;
                    //Удаляем огурчика-Рика
                    cucumber_rick.x = 0;
                    cucumber_rick.y = 0;
                    canvas1.Children.Remove(cucumber_rick.image);
                }

            //перерисовываем экран
            UpdateField();
        }

        // Обработчик нажатия на кнопку клавиатуры
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    head.direction = Head.Direction.UP;
                    break;
                case Key.Down:
                    head.direction = Head.Direction.DOWN;
                    break;
                case Key.Left:
                    head.direction = Head.Direction.LEFT;
                    break;
                case Key.Right:
                    head.direction = Head.Direction.RIGHT;
                    break;
            }
        }

        // Обработчик нажатия кнопки "Start"
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            // обнуляем счет
            score = 0;
            // обнуляем змею
            snake.Clear();
            // очищаем канвас
            canvas1.Children.Clear();
            // скрываем надпись "Game Over"
            tbGameOver.Visibility = Visibility.Hidden;
            
            // добавляем поле на канвас
            canvas1.Children.Add(field.image);
            // создаем новое яблоко и добавлем его
            apple = new Apple(snake);
            canvas1.Children.Add(apple.image);

            // создаем голову
            head = new Head();
            snake.Add(head);
            canvas1.Children.Add(head.image);
            
            //запускаем таймер
            moveTimer.Start();
            moveTimer2.Start();
            UpdateField();
            key = 1;
        }
        
        public class Entity
        {
            protected int m_width;
            protected int m_height;
            
            Image m_image;
            public Entity(int w, int h, string image)
            {
                m_width = w;
                m_height = h;
                m_image = new Image();
                m_image.Source = (new ImageSourceConverter()).ConvertFromString(image) as ImageSource;
                m_image.Width = w;
                m_image.Height = h;
            }

            public Image image
            {
                get
                {
                    return m_image;
                }
            }
        }

        public class PositionedEntity : Entity
        {
            protected int m_x;
            protected int m_y;
            public PositionedEntity(int x, int y, int w, int h, string image)
                : base(w, h, image)
            {
                m_x = x;
                m_y = y;
            }

            public virtual void move() { }

            public int x
            {
                get
                {
                    return m_x;
                }
                set
                {
                    m_x = value;
                }
            }

            public int y
            {
                get
                {
                    return m_y;
                }
                set
                {
                    m_y = value;
                }
            }
        }

        public class Apple : PositionedEntity
        {
            List<PositionedEntity> m_snake;
            public Apple(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/fruit.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random();
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);

            }
        }

        public class Banan : PositionedEntity
        {
            List<PositionedEntity> m_snake;
            public Banan(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/banan.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random(DateTime.Now.Second);
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);
            }
        }

        public class Cucumber : PositionedEntity
        {
            List<PositionedEntity> m_snake;
            public Cucumber(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/cucumber.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random(DateTime.Now.Minute);
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);

            }
        }

        public class Cucumber_Rick : PositionedEntity //////
        {
            List<PositionedEntity> m_snake;
            public Cucumber_Rick(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/cucumber_rick.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random(DateTime.Now.Millisecond);
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);
            }
        }
        /// 
        public class Head : PositionedEntity
        {
            public enum Direction
            {
                RIGHT, DOWN, LEFT, UP, NONE
            };

            Direction m_direction;

            public Direction direction {
                set
                {
                    m_direction = value;
                    RotateTransform rotateTransform = new RotateTransform(90 * (int)value);
                    image.RenderTransform = rotateTransform;
                }
            }

            public Head()
                : base(280, 280, 40, 40, "pack://application:,,,/Resources/head.png")
            {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                m_direction = Direction.NONE;
            }

            public override void move()
            {
                switch (m_direction)
                {
                    case Direction.DOWN:
                        y += 40;
                        break;
                    case Direction.UP:
                        y -= 40;
                        break;
                    case Direction.LEFT:
                        x -= 40;
                        break;
                    case Direction.RIGHT:
                        x += 40;
                        break;
                }
            }
        }

        public class BodyPart : PositionedEntity
        {
            PositionedEntity m_next;
            public BodyPart(PositionedEntity next)
                : base(next.x, next.y, 40, 40, "pack://application:,,,/Resources/body.png")
            {
                m_next = next;
            }

            public override void move()
            {
                x = m_next.x;
                y = m_next.y;
            }
        }
    }
}
