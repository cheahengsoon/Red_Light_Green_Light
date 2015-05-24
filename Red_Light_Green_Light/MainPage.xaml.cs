/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Red_Light_Green_Light
{
    public sealed partial class MainPage : Page
    {

        private int REDStatus = 0;
        private int GREENStatus = 1;
        private bool GO = true;
        private const int RED_PIN = 27;
        private const int GREEN_PIN = 22;
        private GpioPin pin1;
        private GpioPin pin2;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();

            Unloaded += MainPage_Unloaded;

            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin1 = null;
                pin2 = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pin1 = gpio.OpenPin(RED_PIN);
            pin2 = gpio.OpenPin(GREEN_PIN);
            pin1.Write(GpioPinValue.High);
            pin2.Write(GpioPinValue.High);
            pin1.SetDriveMode(GpioPinDriveMode.Output);
            pin2.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            // Cleanup
            pin1.Dispose();
            pin2.Dispose();
        }

        private void FlipRED()
        {
            if (REDStatus == 0)
            {
                REDStatus = 1;
                if (pin1 != null)
                {
                    // to turn on the LED, we need to push the pin 'low'
                    pin1.Write(GpioPinValue.Low);
                }
                LED.Fill = redBrush;
            }
            else
            {
                REDStatus = 0;
                if (pin1 != null)
                {
                    pin1.Write(GpioPinValue.High);
                }
                // LED.Fill = grayBrush;
            }
        }

        private void TurnOffRED()
        {
            if (REDStatus == 0)
            {
                FlipRED();
            }
        }

        private void FlipGREEN()
        {
            if (GREENStatus == 0)
            {
                GREENStatus = 1;
                if (pin2 != null)
                {
                    // to turn on the LED, we need to push the pin 'low'
                    pin2.Write(GpioPinValue.Low);
                }
                LED.Fill = greenBrush;
            }
            else
            {
                GREENStatus = 0;
                if (pin2 != null)
                {
                    pin2.Write(GpioPinValue.High);
                }
                // LED.Fill = grayBrush;
            }
        }

        private void TurnOffGREEN()
        {
            if (GREENStatus == 0)
            {
                FlipGREEN();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            FlipRED();
            FlipGREEN();
        }

        private void Delay_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (timer == null)
            {
                return;
            }
            if (e.NewValue == Delay.Minimum)
            {
                DelayText.Text = "Stopped";
                timer.Stop();
                TurnOffRED();
                TurnOffGREEN();
            }
            else
            {
                DelayText.Text = e.NewValue + "ms";
                timer.Interval = TimeSpan.FromMilliseconds(e.NewValue);
                timer.Start();
            }
        }

        private void buttonSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (GO == true)
            {
                GO = false;
                DelayText.Text = "Stopped";
                timer.Stop();
                TurnOffRED();
                TurnOffGREEN();
                LED.Fill = grayBrush;
            }
            else
            {
                GO = true;
                DelayText.Text = timer.Interval.Milliseconds.ToString() + "ms";
                timer.Start();
                REDStatus = 0;
                GREENStatus = 1;
            }
        }
    }
}
