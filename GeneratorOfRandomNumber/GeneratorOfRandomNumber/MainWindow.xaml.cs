using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GeneratorOfRandomNumber
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<int> _randomNumbers;
        private string _uri = "https://www.random.org/sequences/";

        public MainWindow()
        {
            InitializeComponent();
        }

        //Task 5. Method cache result into a local queue and return numbers from it. If queue becomes empty –
        //make an async request to API to populate queue, and return numbers from BCL`s Random generator
        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            _randomNumbers = new Queue<int>();
            _uri += "?min=" + MinNumberTextBox.Text + "&max=" + MaxNumberTextBox.Text +
                    "&col=1&format=plain&rnd=new";
            var response = await client.GetAsync(_uri);

            MinNumberTextBox.Text = null;
            MaxNumberTextBox.Text = null;
            TextOutputBlock.Text = null;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var responseText = await response.Content.ReadAsStringAsync();
                    responseText = responseText.Replace("\n", " ");
                    var numbers = responseText.Split(' ');
                    foreach (var number in numbers.Where(number => number != ""))
                    {
                        _randomNumbers.Enqueue(Convert.ToInt32(number));
                        Dispatcher?.BeginInvoke(DispatcherPriority.Normal,
                            (Action) (() => { TextOutputBlock.Text += _randomNumbers.Dequeue() + ", "; }));
                    }
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    TextOutputBlock.Text = "Sorry! Service unavailable!";
                    break;
            }
        }

        private void ClearTextButton_Click(object sender, RoutedEventArgs e)
        {
            MinNumberTextBox.Text = null;
            MaxNumberTextBox.Text = null;
            TextOutputBlock.Text = null;
        }

        //The method, which shows the work of the class MyMutex
        private async void TestMyMutexButton_Click(object sender, RoutedEventArgs e)
        {
            var mut1 = new MyMutex();
            var mut2 = new MyMutex();
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() =>
            {
                TextOutputBlock.Text = "ThreadOne, executing ThreadMethod, " +
                                       $"is {Thread.CurrentThread.ManagedThreadId} from the thread pool.";
            }));

            // Wait until it is safe to enter.
            await mut1.Lock();

            // Simulate some work.
            Thread.Sleep(1000);
            await
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action) (() => { TextOutputBlock.Text += "\nWork critical code..."; }));

            // Release the Mutex.
            mut1.Release();

            //Task 4. using(await mutex.LockSection()) {//critical code}
            using (await mut2.LockSection())
            {
                // Simulate some work.
                Thread.Sleep(1000);
                await
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)
                            (() =>
                            {
                                TextOutputBlock.Text +=
                                    "\nWork critical code in block using(await mut.LockSection()){}...";
                            }));
            }
        }
    }
}
