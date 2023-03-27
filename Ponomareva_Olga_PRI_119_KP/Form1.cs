using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.DevIl;
using Tao.FreeGlut;
using Tao.OpenGl;

namespace Ponomareva_Olga_PRI_119_KP
{
    public partial class Form1 : Form
    {
        float global_time = 0;
        float deltaColor;
        double translateX = -9, translateY = -60, translateZ = -10;
        double boxTranslateX = 37, boxTranslateY = 145, boxTranslateZ = 28;
        private float cleanerTranslateX = 40;
        private float cleanerTranslateY = 100;
        private readonly float cleanerTranslateZ = 1;
        double angle = 5, angleX = -70, angleY = 0, angleZ = 0;
        double green, greenX, greenY, greenZ;
        double rain;
        bool lampBoom, clinerBoom;

        int imageId; uint mGlTextureObject;
        // адрес изображения
        string url = "texture/cat.jpg";


        RainDelta rainDelta = new RainDelta(new Deltas[6]);

        RainDelta rainDelta2 = new RainDelta(new Deltas[6]);


        // эккземпляра класса Explosion
        private Explosion BOOOOM_1 = new Explosion(1, 10, 1, 300, 500);
        private void button1_Click(object sender, EventArgs e)
        {
            lampBoom = true;

            deltaColor = 0.2f;
            AnT.Focus();
            button1.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            deltaColor = 0;
            button1.Enabled = true;
        }



        double sizeX = 1, sizeY = 1, sizeZ = 1;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
                cameraSpeed = (double)numericUpDown1.Value;
            AnT.Focus();
        }

        double cameraSpeed;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация Glut
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);
            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);
            // отчитка окна
            Gl.glClearColor(255, 255, 255, 1);
            // установка порта вывода в соответствии с размерами элемента anT
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);
            // настройка проекции
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 800);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            // настройка параметров OpenGL для визуализации
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            comboBox2.SelectedIndex = 0;
            cameraSpeed = 1;
            numericUpDown1.Value = 1;

            // создаем изображение с идентификатором imageid 
            Il.ilGenImages(1, out imageId);
            // делаем изображение текущим 
            Il.ilBindImage(imageId);


            // пробуем загрузить изображение 
            if (Il.ilLoadImage(url))
            {

                // если загрузка прошла успешно 
                // сохраняем размеры изображения 
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

                // определяем число бит на пиксель 
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);

                switch (bitspp) // в зависимости от полученного результата 
                {
                    // создаем текстуру, используя режим gl_rgb или gl_rgba 
                    case 24:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }

                // очищаем память 
                Il.ilDeleteImages(1, ref imageId);
            }

            for (int i = 0; i < 6; i++)
            {
                rainDelta.rains[i] = new Deltas(3, 35);
                rainDelta2.rains[i] = new Deltas(3, 35);
            }
            rainDelta.rains[1] = new Deltas(3, 30);
            rainDelta.rains[2] = new Deltas(3, 25);
            rainDelta.rains[3] = new Deltas(3, 20);

            rainDelta2.rains[1] = new Deltas(3, 30);
            rainDelta2.rains[2] = new Deltas(3, 25);
            rainDelta2.rains[3] = new Deltas(3, 20);


            RenderTimer.Start();
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            global_time += (float)RenderTimer.Interval / 1000;
            Draw();
        }

        private void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            Gl.glColor3f(1.0f - deltaColor, 0, 0);
            Gl.glPushMatrix();
            Gl.glTranslated(translateX, translateY, translateZ);
            Gl.glRotated(angleX, 1, 0, 0);
            Gl.glRotated(angleY, 0, 1, 0);
            Gl.glRotated(angleZ, 0, 0, 1);
            BOOOOM_1.Calculate(global_time);


            //Стол
            Gl.glPushMatrix();
            Gl.glTranslated(10, 145, 39);
            Gl.glScaled(9, 7, 0.2);
            Gl.glColor3f(0.4f - deltaColor, 0.2f - deltaColor, 0);
            Glut.glutSolidCube(10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireCube(10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-29, 145, 20);
            Gl.glScaled(0.6, 15, 9);
            Gl.glColor3f(0.4f - deltaColor, 0.2f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(49, 145, 20);
            Gl.glScaled(0.6, 15, 9);
            Gl.glColor3f(0.4f - deltaColor, 0.2f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(boxTranslateX, boxTranslateY, boxTranslateZ);
            Gl.glScaled(5, 15, 2.5);
            Gl.glColor3f(0.4f - deltaColor, 0.2f - deltaColor, 0);
            Glut.glutSolidCube(4);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(5f);
            Glut.glutWireCube(4);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(boxTranslateX, boxTranslateY, 33.5);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor3d(1 - deltaColor, 1 - deltaColor, 0.5 - deltaColor);
            Gl.glVertex3d(-10, -30, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(3, -30, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(3, 0, 0);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(-10, 0, 0);
            Gl.glTexCoord2f(1, 0);
            Gl.glEnd();
            Gl.glPopMatrix();

            //ноутбук
            Gl.glPushMatrix();
            Gl.glTranslated(-10, 145, 40.8);
            Gl.glScaled(2, 2, 0.1);
            Gl.glColor3f(0.3f - deltaColor, 0.3f - deltaColor, 0.3f - deltaColor);
            Glut.glutSolidCube(10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireCube(10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-10, 156, 48);
            Gl.glScaled(2, 0.1, 1.5);
            Gl.glColor3f(0.3f - deltaColor, 0.3f - deltaColor, 0.3f - deltaColor);
            Glut.glutSolidCube(10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireCube(10);
            Gl.glPopMatrix();

            // включаем режим текстурирования 
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            // включаем режим текстурирования, указывая идентификатор mGlTextureObject 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);

            // сохраняем состояние матрицы 
            Gl.glPushMatrix();
            Gl.glTranslated(-10, 155, 47);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor3d(0, 0, 0);
            Gl.glVertex3d(-9, 0, -5);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(9, 0, -5);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(9, 0, 8);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(-9, 0, 8);
            Gl.glTexCoord2f(1, 0);
            Gl.glEnd();
            // возвращаем матрицу 
            // отключаем режим текстурирования 
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            //клавиатура
            double buttonDeltaX = 0;
            double buttonDeltaY = 0;
            double buttonDeltaZ = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    Gl.glPushMatrix();
                    Gl.glTranslated(-18 + buttonDeltaX, 152 + buttonDeltaY, 41 + buttonDeltaZ);
                    Gl.glScaled(0.1, 0.1, 0.15);
                    Gl.glColor3f(0.2f - deltaColor, 0.2f - deltaColor, 0.2f - deltaColor);
                    Glut.glutSolidCube(10);
                    Gl.glPopMatrix();

                    buttonDeltaX += 1.6;
                }
                buttonDeltaX = 0;
                buttonDeltaY -= 2.4;
            }

            //документы
            Gl.glPushMatrix();
            Gl.glTranslated(40, 145, 41.5);
            Gl.glRotatef(-15, 0, 0, 1);
            Gl.glScaled(1.5, 3, 0.3);
            Gl.glColor3f(0.1f - deltaColor, 0.1f - deltaColor, 0.1f - deltaColor);
            Glut.glutSolidCube(10);
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(2f);
            Glut.glutWireCube(10);
            Gl.glPopMatrix();

            //окно
            double rainX = 3; //не меньше 0
            double rainZ = 35; //не меньше 5

            rain -= 0;

            for (int i = 0; i < 6; i++)
            {
                if ((rainDelta.rains[i].z - 3 - rain) <= 5)
                {
                    rainDelta.rains[i].x = rainX;
                    rainDelta.rains[i].z = rainZ;
                    rain = 0.1;
                    for (int j = 0; j < 7; j++)
                    {
                        Gl.glPushMatrix();
                        Gl.glTranslated(20, 200, 50);
                        Gl.glLineWidth(1);
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glColor3d(0, 0, 1);
                        Gl.glVertex3d(rainDelta.rains[i].x, 0, rainDelta.rains[i].z - rain);
                        Gl.glVertex3d(rainDelta.rains[i].x, 0, rainDelta.rains[i].z - 3 - rain);

                        Gl.glEnd();
                        Gl.glPopMatrix();
                        rainDelta.rains[i].x += 5;
                    }
                }
                else
                {
                    for (int j = 0; j < 7; j++)
                    {
                        Gl.glPushMatrix();
                        Gl.glTranslated(20, 200, 50);
                        Gl.glLineWidth(1);
                        Gl.glBegin(Gl.GL_LINES);
                        Gl.glColor3d(0, 0, 1);
                        Gl.glVertex3d(rainDelta.rains[i].x, 0, rainDelta.rains[i].z - rain);
                        Gl.glVertex3d(rainDelta.rains[i].x, 0, rainDelta.rains[i].z - 3 - rain);
                        Gl.glEnd();
                        Gl.glPopMatrix();

                        rainDelta.rains[i].x += 5;
                    }
                }
                if (i % 2 != 0)
                {
                    rainDelta.rains[i].z -= 5;
                    rainDelta.rains[i].x = 5;
                }
                else
                {
                    rainDelta.rains[i].z -= 5;
                    rainDelta.rains[i].x = 3;
                }
            }

            Gl.glPushMatrix();
            Gl.glTranslated(20, 200, 50);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glColor3d(0.7, 1.96, 2.50);
            Gl.glVertex3d(0, 0, 5);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(37, 0, 5);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(37, 0, 35);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 35);
            Gl.glTexCoord2f(1, 0);
            Gl.glEnd();

            Gl.glLineWidth(10);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glColor3d(0.4 - deltaColor, 0.2 - deltaColor, 0);

            Gl.glVertex3d(0, 0, 5);
            Gl.glVertex3d(38, 0, 5);

            Gl.glVertex3d(37, 0, 5);
            Gl.glVertex3d(37, 0, 36);

            Gl.glVertex3d(37, 0, 36);
            Gl.glVertex3d(0, 0, 36);

            Gl.glVertex3d(0, 0, 37);
            Gl.glVertex3d(0, 0, 4);
            Gl.glEnd();

            Gl.glPopMatrix();

            //пылесос
            Gl.glPushMatrix();
            Gl.glTranslated(cleanerTranslateX, cleanerTranslateY, cleanerTranslateZ);
            Gl.glColor3f(0.5f - deltaColor, 0, 1.56f - deltaColor);
            Glut.glutSolidCylinder(7, 2, 10, 10);
            Gl.glLineWidth(2f);
            Gl.glColor3f(0, 0, 0);
            Glut.glutWireCylinder(7, 2, 10, 10);
            Gl.glPopMatrix();

            //label1.Text = Convert.ToString(cleanerTranslateX);
            //label16.Text = Convert.ToString(cleanerTranslateY);

            if (clinerBoom)
            {
                // устанавливаем новые координаты взрыва
                BOOOOM_1.SetNewPosition(cleanerTranslateX, cleanerTranslateY, cleanerTranslateZ);
                // случайную силу
                BOOOOM_1.SetNewPower(70);
                // и активируем сам взрыв
                BOOOOM_1.Boooom(global_time);
                clinerBoom = false;
            }

            //горшок
            Gl.glPushMatrix();
            Gl.glTranslated(20, 140, 40.2);
            Gl.glColor3f(1.2f - deltaColor, 0, 0);
            Glut.glutSolidCylinder(4, 7, 10, 10);
            Gl.glLineWidth(2f);
            Gl.glColor3f(0, 0, 0);
            Glut.glutWireCylinder(4, 7, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(20, 140, 47);
            Gl.glColor3f(1.2f - deltaColor, 0, 0);
            Glut.glutSolidCylinder(4.5, 1, 10, 10);
            Gl.glLineWidth(2f);
            Gl.glColor3f(0, 0, 0);
            Glut.glutWireCylinder(4.5, 1, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(20 + greenX, 140 + greenZ, 47);
            Gl.glColor3f(0, 1.56f - deltaColor, 0.3f - deltaColor);
            Glut.glutSolidCylinder(0.1, 7, 10, 10);
            Gl.glPopMatrix();

            //лапма
            Gl.glPushMatrix();
            Gl.glTranslated(-50, 150, 0);
            Gl.glColor3f(1.22f - deltaColor, 0, 1.56f - deltaColor);
            Glut.glutSolidCylinder(7, 2, 10, 10);
            Gl.glLineWidth(2f);
            Gl.glColor3f(0, 0, 0);
            Glut.glutWireCylinder(7, 2, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslated(-50, 150, 0);
            Gl.glColor3f(1.22f - deltaColor, 0, 1.56f - deltaColor);
            Glut.glutSolidCylinder(1, 55, 10, 10);
            Gl.glLineWidth(2f);
            Gl.glColor3f(0, 0, 0);
            Glut.glutWireCylinder(1, 55, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            
            circle(-50, 150, 55, 8, 0.5f);
            Point[] lamp = new Point[40];
            double grad = Math.PI * 2 / 40;
            for (int i = 0; i < 40; i++)
            {
                lamp[i] = new Point();
                lamp[i].x = Math.Cos(grad * i) * 1.2 - 50;
                lamp[i].y = Math.Sin(grad * i) * 1.2 + 150;
                lamp[i].z = 65;
            }
            Gl.glLineWidth(3f);
            lamp = cilinder(lamp, -50, 150, 62, 6, 1.22f - deltaColor, 0, 1.56f - deltaColor);
            lamp = cilinder(lamp, -50, 150, 58.5, 7, 1.22f - deltaColor, 0, 1.56f - deltaColor);
            lamp = cilinder(lamp, -50, 150, 55, 9, 1.22f - deltaColor, 0, 1.56f - deltaColor);
            Gl.glPopMatrix();

            if (lampBoom)
            {
                // устанавливаем новые координаты взрыва
                BOOOOM_1.SetNewPosition(-50, 150, 60);
                // случайную силу
                BOOOOM_1.SetNewPower(70);
                // и активируем сам взрыв
                BOOOOM_1.Boooom(global_time);
                lampBoom = false;
            }


            Gl.glPushMatrix();
            //пол
            Gl.glColor3f(1.5f - deltaColor, 0.9f - deltaColor, 0.6f - deltaColor);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(200, 200, 0);
            Gl.glVertex3d(-200, 200, 0);
            Gl.glVertex3d(-200, -10, 0);
            Gl.glVertex3d(200, -10, 0);
            Gl.glEnd();
            double line = 0;
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(3f);
            do
            {
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(200 - line, 195, 1);
                Gl.glVertex3d(200 - line, -10, 1);
                Gl.glEnd();
                line += 10;
            }
            while (line < 400);
            Gl.glPopMatrix();

            Gl.glPopMatrix();
            Gl.glFlush();
            AnT.Invalidate();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                angle = 5;
                angleX = -70;
                angleY = 0;
                angleZ = 0;
                translateX = -9;
                translateY = -60;
                translateZ = -10;
            }
            if (comboBox2.SelectedIndex == 1)
            {
                angle = 5;
                angleX = -80;
                angleY = 0;
                angleZ = 0;
                translateX = 10;
                translateY = -80;
                translateZ = 90;
            }
            if (comboBox2.SelectedIndex == 2)
            {
                angle = 5;
                angleX = -80;
                angleY = 0;
                angleZ = 15;
                translateX = -5;
                translateY = -80;
                translateZ = 80;
            }
            AnT.Focus();
        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && boxTranslateY > 115)
            {
                boxTranslateY -= 1;
            }
            if (e.KeyCode == Keys.Q && boxTranslateY < 145)
            {
                boxTranslateY += 1;
            }

            if (e.KeyCode == Keys.NumPad4 && cleanerTranslateX >= -190)
            {
                cleanerTranslateX -= 1;
                if ((cleanerTranslateX == -21 && (cleanerTranslateY < 183 && cleanerTranslateY >= 108)) ||
                    cleanerTranslateX == 58 && (cleanerTranslateY < 183 && cleanerTranslateY >= 108))
                {
                    clinerBoom = true;
                    cleanerTranslateX += 5;
                }
            }
            if (e.KeyCode == Keys.NumPad6 && cleanerTranslateX <= 190)
            {
                cleanerTranslateX += 1;
                if ((cleanerTranslateX == -37 && (cleanerTranslateY < 183 && cleanerTranslateY >= 108)) ||
                    cleanerTranslateX == 41 && (cleanerTranslateY < 183 && cleanerTranslateY >= 108) ||
                    cleanerTranslateX == -64 && cleanerTranslateY < 164 && cleanerTranslateY >= 142)
                {
                    clinerBoom = true;
                    cleanerTranslateX -= 5;
                }
            }
            if (e.KeyCode == Keys.NumPad8 && cleanerTranslateY <= 190)
            {
                cleanerTranslateY += 1;
                if ((cleanerTranslateX >= -35 && cleanerTranslateX <= -38) && cleanerTranslateY == 110 ||
                    (cleanerTranslateX == -34 || cleanerTranslateX == -23) && cleanerTranslateY == 109 ||
                    cleanerTranslateX == -33 && cleanerTranslateY == 107 ||
                    (cleanerTranslateX >= -32 && cleanerTranslateX <= -24) && cleanerTranslateY == 107 ||
                    cleanerTranslateX >= -31 && cleanerTranslateX <= -26 && cleanerTranslateY == 107 ||
                    (cleanerTranslateX >= 41 && cleanerTranslateX <= 58) && cleanerTranslateY == 107 ||
                    (cleanerTranslateX >= -60 && cleanerTranslateX <= -38) && cleanerTranslateY == 135)
                {
                    clinerBoom = true;
                    cleanerTranslateY -= 5;
                }
            }
            if (e.KeyCode == Keys.NumPad2 && cleanerTranslateY >= 0)
            {
                cleanerTranslateY -= 1;
                if (cleanerTranslateY == 163 && cleanerTranslateX >= -63 && cleanerTranslateX <= -39)
                {
                    clinerBoom = true;
                    cleanerTranslateY += 5;
                }
            }

            if (e.KeyCode == Keys.X)
            {
                translateZ -= cameraSpeed;
            }
            if (e.KeyCode == Keys.E)
            {
                translateZ += cameraSpeed;
            }
            if (e.KeyCode == Keys.A)
            {
                translateX += cameraSpeed;
            }
            if (e.KeyCode == Keys.D)
            {
                translateX -= cameraSpeed;
            }
            if (e.KeyCode == Keys.W)
            {
                translateY -= cameraSpeed;
            }
            if (e.KeyCode == Keys.S)
            {
                translateY += cameraSpeed;
            }
        }

        private Point[] cilinder(Point[] circle0, double x1, double y1, double z1, double R1, float red, float green, float blue)
        {
            Point[] circle1 = new Point[40];
            int count = 40;
            double grad = Math.PI * 2 / count;
            for (int i = 0; i < count; i++)
            {
                circle1[i] = new Point();
                circle1[i].x = Math.Cos(grad * i) * R1 + x1;
                circle1[i].y = Math.Sin(grad * i) * R1 + y1;
                circle1[i].z = z1;
            }
            Gl.glColor3f(red, green, blue);
            for (int i = 0; i < count - 1; i++)
            {
                Gl.glColor3f(red, green, blue);
                Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                Gl.glVertex3d(circle0[i].x, circle0[i].y, circle0[i].z);
                Gl.glVertex3d(circle1[i].x, circle1[i].y, circle1[i].z);
                Gl.glVertex3d(circle1[i + 1].x, circle1[i + 1].y, circle1[i + 1].z);
                Gl.glVertex3d(circle0[i + 1].x, circle0[i + 1].y, circle0[i + 1].z);
                Gl.glEnd();
                Gl.glColor3f(0, 0, 0);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(circle0[i].x, circle0[i].y, circle0[i].z);
                Gl.glVertex3d(circle0[i + 1].x, circle0[i + 1].y, circle0[i + 1].z);
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINE_STRIP);
                Gl.glVertex3d(circle1[i].x, circle1[i].y, circle1[i].z);
                Gl.glVertex3d(circle1[i + 1].x, circle1[i + 1].y, circle1[i + 1].z);
                Gl.glEnd();
            }
            Gl.glColor3f(red, green, blue);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glVertex3d(circle0[count - 1].x, circle0[count - 1].y, circle0[count - 1].z);
            Gl.glVertex3d(circle1[count - 1].x, circle1[count - 1].y, circle1[count - 1].z);
            Gl.glVertex3d(circle1[0].x, circle1[0].y, circle1[0].z);
            Gl.glVertex3d(circle0[0].x, circle0[0].y, circle0[0].z);
            Gl.glEnd();
            Gl.glColor3f(0, 0, 0);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(circle0[count - 1].x, circle0[count - 1].y, circle0[count - 1].z);
            Gl.glVertex3d(circle0[0].x, circle0[0].y, circle0[0].z);
            Gl.glEnd();
            Gl.glBegin(Gl.GL_LINE_STRIP);
            Gl.glVertex3d(circle1[count - 1].x, circle1[count - 1].y, circle1[count - 1].z);
            Gl.glVertex3d(circle1[0].x, circle1[0].y, circle1[0].z);
            Gl.glEnd();
            return circle1;
        }


        private Point[] circle(double x, double y, double z, double R, float width)
        {
            Point[] krug = new Point[40];
            double grad = 360 / 40;
            double a, b;
            Gl.glLineWidth(width);
            Gl.glColor3f(0, 0, 0);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < 40; i++)
            {
                a = -Math.Cos(grad * i) * R + x;
                b = -Math.Sin(grad * i) * R + y;
                krug[i] = new Point(a, b, z);
                Gl.glVertex3d(a, b, z);
            }
            Gl.glEnd();
            return krug;
        }

        public class RainDelta
        {
            public Deltas[] rains{get;set;}

            public RainDelta(Deltas[] d)
            {
                this.rains = d;
            }


        }

        public class Deltas
        {
            public double x { get; set; }
            public double z { get; set; }

            public Deltas(double x, double z)
            {
                this.x = x;
                this.z = z;
            }

        }

        // создание текстуры в памяти openGL 
        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {
            // идентификатор текстурного объекта 
            uint texObject;

            // генерируем текстурный объект 
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей 
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры 
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            // создаем RGB или RGBA текстуру 
            switch (Format)
            {
                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;
            }

            // возвращаем идентификатор текстурного объекта 
            return texObject;
        }

    }
}
