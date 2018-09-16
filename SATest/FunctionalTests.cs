using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using SA_EF;
using System.Threading;
using System.Windows.Automation;
using TestStack.White;
using TestStack.White.Configuration;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Actions;
using TestStack.White.UIItems.Custom;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;

namespace SATest
{
    [TestClass]
    public class UITests
    {
        //dummy user just to get into system, has no rights whatever
        string testuser1 = "catest";
        //users with read write rights
        string testuser2 = "catest1";
        string testuser3 = "catest2";
        //pwd common for all
        string testpwd1 = "ca123_CA123";
        static string appPath = String.Empty;

        static Application app;
        #region Test Ctor & ~
        [TestInitialize]
        public void UITestsInit()
        {
            if (!File.Exists(appPath))
            {
                if (!File.Exists(Properties.Settings.Default.AppPath))
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.Filter = "Executable files|*.exe";
                    openFileDialog.Title = "Задайте путь к программе";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        appPath = openFileDialog.FileName;
                        Properties.Settings.Default.AppPath = openFileDialog.FileName;
                        Properties.Settings.Default.Save();
                    }
                    else Assert.Fail("Application not found");
                }
                else appPath = Properties.Settings.Default.AppPath;
            }
            app = Application.Launch(appPath);
            CoreAppXmlConfiguration.Instance.BusyTimeout = 3000;
        }

        [TestCleanup]
        public void UITestcleanup()
        {
            app?.Close();
        }

        protected bool Authorize(string uName, string pwd)
        {
            if (app == null) return false;
            else
            {
                Window window = app.GetWindow("Авторизация");
                TextBox tbUserName = window.Get<TextBox>("tbUserName");
                tbUserName.BulkText = uName;
                TextBox tbPwd = window.Get<TextBox>("pbPassword");
                tbPwd.Enter(pwd);
                Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
                btn.Click();
                app.WaitWhileBusy();
                var wnds = app.GetWindows();
                return wnds.Find(x => x.Name.StartsWith("Расчет")) != null;
            }
        }
        #endregion Test Ctor & ~
        #region Authorization
        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-01")]
        public void AuthorizationPositive()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
        }
    
        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-02")]
        public void AuthorizationNegativeWrongUser()
        {
            Assert.IsFalse(Authorize(new string('a', 10), new string('g', 10)), "Wrong user came in!");
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Wrong user came in!");
        }

        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-03")]
        public void AuthorizationNegativeWrongUser3Attempts()
        {
            for (int i = 1; i <= 3; i++)
            {
                Authorize(new string('a', 10), new string('g', 10));
                var wnds = app.GetWindows();
                if (i != 3)
                {
                    Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Wrong user came in!");
                    wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
                }
                else
                {
                    Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка авторизации")), "Wrong user came in!");
                    wnds[0].Get<Button>(SearchCriteria.ByAutomationId("2")).Click();
                }
            }
        }
        #endregion Authorization
        #region Samples
        [TestMethod]
        [Description("CA-02-01")]
        public void SamplesListLoadSamplePositive()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
        }

        [TestMethod]
        [Description("CA-02-02")]
        public void SamplesListLoadSamplesFails_UnAuthorizedUser()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
        }

        [TestMethod]
        [Description("CA-02-03")]
        public void SamplesListFilterSamples()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            wnds[0].Get<Label>(SearchCriteria.ByAutomationId("txtFilters")).RightClick();
            PopUpMenu pop;
            Assert.IsNotNull(pop = wnds[0].Popup);
            pop.ItemBy(SearchCriteria.ByAutomationId("miClearFilter")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            int count = lbS.Items.Count;
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnFilter")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Отбор образцов")), "Filter cannot be set");
            DateTimePicker dp = null;
            Assert.IsNotNull(dp = wnds[0].Get<DateTimePicker>(SearchCriteria.ByAutomationId("dpEndDate")));
            dp.SetDate(DateTime.Now.AddMonths(-1),DateFormat.DayMonthYear);
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.AreNotEqual<int>(count, lbS.Items.Count,"Filter doesn't work or conditions are wrong");
        }

        [TestMethod]
        [Description("CA-02-04")]
        public void SamplesListAddSample()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Добавить…")).Click();
            wnds = app.GetWindows();
            Window wnd = null;
            Assert.IsNotNull(wnd = wnds.Find(x => x.Name.StartsWith("Внести новый образец")), "Adding unavailable!");
            string labNumber = RandomString.GetRandomString();
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbLabNumber")).SetValue(labNumber);
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).Enter(RandomString.GetRandomString());
            wnd.Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.Find(x => x.Name.Contains(labNumber)));
        }

        [TestMethod]
        [Description("CA-02-05")]
        public void SamplesListDeleteSample()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            Assert.IsNotNull(lbS);
            string lastSample = null;
            Assert.IsNotNull(lastSample = lbS.Items.FindLast(p => true).Name);
            lbS.Items.FindLast(p => true).Select();
            wnds[0].Get<Button>(SearchCriteria.ByText("Удалить")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "Deletion of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNull(lbS.Items.Find(x => x.Name == lastSample));
        }

        [TestMethod]
        [Description("CA-02-06")]
        public void SamplesListViewSampleParameters()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).DoubleClick();
            wnds = app.GetWindows();
            Window window = null;
            Assert.IsNotNull(window = wnds.Find(x => x.Name.StartsWith("Редактировать")), "View of sample is unavailable");
            window.Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
        }

        [TestMethod]
        [Description("CA-02-07")]
        public void SamplesListEditSampleParameters()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).DoubleClick();
            wnds = app.GetWindows();
            Window window = null;
            Assert.IsNotNull(window = wnds.Find(x => x.Name.StartsWith("Редактировать")), "View of sample is unavailable");
            string newDesc = RandomString.GetRandomString();
            DateTime dt = DateTime.Now.AddDays(-2); // Set the day before yesterday
            window.Get<DateTimePicker>(SearchCriteria.ByAutomationId("dpSamplingDate")).SetDate(dt,DateFormat.DayMonthYear);
            window.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).SetValue(newDesc);
            window.Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS.Items.Find(p => p.Name.Contains(newDesc) 
                && p.Name.Contains(dt.ToString("dd-MM-yyyy"))));
        }

        [TestMethod]
        [Description("CA-02-08")]
        public void SamplesListSamplesFilterByDB()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).DoubleClick();
            wnds = app.GetWindows();
            Window window = null;
            Assert.IsNotNull(window = wnds.Find(x => x.Name.StartsWith("Редактировать")), "View of sample is unavailable");
            string labNumber = window.Get<TextBox>(SearchCriteria.ByAutomationId("tbLabNumber")).Text;
            string desc = window.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).Text;
            window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);
            wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Файл")).SubMenu(SearchCriteria.ByAutomationId("miChangeUser")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Авторизация")), "Change user failed!");
            Assert.IsTrue(Authorize(testuser3, testpwd1), "Вход не осуществлен!");
            wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNull(lbS.Items.Find(x => x.Name.Contains(labNumber) && x.Name.Contains(desc)));
        }
        #endregion Samples
        #region Analyses
        [TestMethod]
        [Description("CA-03-01")]
        public void AnalysesAddNew()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).RightClick();
            PopUpMenu pop = wnds[0].Popup;

            ListView list;
            //remember editing analyses MI state for further needs
            bool wasEditingEnabled = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Enabled;

            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "List of analyses unavailable!");
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows.FindLast(p => true).DoubleClick();
            Thread.Sleep(1000);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Описание")), "Description of samples unavailable!");
            string sampleDescription = RandomString.GetRandomString();
            wnds[0].Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).BulkText = sampleDescription;
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();

            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).RightClick();
            pop = wnds[0].Popup;

            if (!wasEditingEnabled) Assert.IsTrue(pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Enabled,
                 "Wasn't added!");
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            Window wnd = wnds[0];
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            List<string> details = new List<string>();
            foreach(ListViewRow v in list.Rows)
            {
                v.DoubleClick();
                Thread.Sleep(500);
                wnds = app.GetWindows();
                details.Add(wnds[0].Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).Text);
                wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);
            }
            Assert.IsNotNull(details.Find(p => p.Contains(sampleDescription)));
            list.Select(details.FindIndex(p => p.Contains(sampleDescription)));
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.DELETE);
            wnds = app.GetWindows();
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
        }

        [TestMethod]
        [Description("CA-03-02")]
        public void AnalysesViewData()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            Window wnd = wnds[0];
            ListView list = null;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
        }

        [TestMethod]
        [Description("CA-03-03")]
        public void AnalysesChangeData()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            Window wnd = wnds[0];
            ListView list = null;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            var r = list.Rows.FindLast(p => true).AsContainer();
            WpfDatePicker z = (r as UIItemContainer).Items.Find(p => p.GetType().Equals(typeof(WpfDatePicker))) as WpfDatePicker;
            //Store new datetime value
            DateTime dt = z.Date.Value.AddDays(-2);
            z.Date = dt;
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            lbS.Items.FindLast(p => true).RightClick();
            pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            wnd = wnds[0];
            list = null;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            r = list.Rows.FindLast(p => true).AsContainer();
            z = (r as UIItemContainer).Items.Find(p => p.GetType().Equals(typeof(WpfDatePicker))) as WpfDatePicker;
            Assert.AreEqual(z.Date, dt);
        }

        [TestMethod]
        [Description("CA-03-04")]
        public void AnalysesDelete()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            ListBox lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(lbS);
            Assert.IsNotNull(lbS.Items.FindLast(p => true).Name, "No samples is available");
            lbS.Items.FindLast(p => true).RightClick();
            PopUpMenu pop = wnds[0].Popup;
            if (!pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Enabled)
                Assert.Fail("No analyses to delete", pop);
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            Window wnd = wnds[0];
            ListView list = null;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            //Remember count of rows (analyses)
            int analysescount = list.Rows.Count;
            list.Rows.FindLast(p => true).Select();
            //find row details
            CADataGridDetails details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter"));
            System.Windows.Rect rc = details.Bounds;
            wnds[0].Mouse.Location = new System.Windows.Point((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2);
            wnds[0].Click();
            wnd.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.DELETE);
            wnds = app.GetWindows();
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnd.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);//close the window
            
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            lbS = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            lbS.Items.FindLast(p => true).RightClick();
            pop = wnds[0].Popup;
            if (analysescount == 1)
            {
                Assert.IsFalse(pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Enabled);
                return;
            }
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")),
               "List of analyses unavailable!");
            wnd = wnds[0];
            list = null;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            Assert.AreEqual(list.Rows.Count, analysescount - 1);
        }

        [TestMethod]
        [Description("CA-03-05")]
        public void CalculateAnalysis()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Select ALL
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            CADataGridDetails details;
            Assert.IsNotNull(details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter")),
                "Cannot find row details");
            Assert.AreEqual(details.GetCheckBoxState(), ToggleState.Off);
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);//Calculate
            wnds[0].Keyboard.Enter("C");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
            Assert.AreEqual(details.GetCheckBoxState(), ToggleState.On);
        }

        [TestMethod]
        [Description("CA-03-06")]
        public void SetAnalysisScheme()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows[0].Click();
            CADataGridDetails details;
            Assert.IsNotNull(details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter")),
                "Cannot find row details");
            Assert.AreEqual(details.GetScheme(), "Хлоридная");//defualt scheme
            details.SetScheme("Карбонатная");       //not yet realized scheme
            Assert.IsFalse(wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Enabled);
        }

        [TestMethod]
        [Description("CA-03-07")]
        public void CompareCalculationResults()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            //Add 2 new analyses
            wnds[0].Mouse.Click(pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses"))
                .GetElement(SearchCriteria.ByAutomationId("IncreaseCommand")).GetClickablePoint());
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Select ALL
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
           
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);//Calculate
            wnds[0].Keyboard.Enter("C");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            wnds.Find(w=>w.Name.StartsWith("Результаты расчета")).Get<Button>().Click();
            wnds = app.GetWindows();
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Deselect and Select ALL again
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);

            CADataGridDetails details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter"));
            System.Windows.Rect rc = details.Bounds;
            details.RightClickAt(new System.Windows.Point((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2));

            wnds = app.GetWindows();
            pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("imCompareResults")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Результаты сравнения")));
        }

        [TestMethod]
        [Description("CA-03-08")]
        public void PrintCalculationResults()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            //Add 2 new analyses
            wnds[0].Mouse.Click(pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses"))
                .GetElement(SearchCriteria.ByAutomationId("IncreaseCommand")).GetClickablePoint());
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Select ALL
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);

            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);//Calculate
            wnds[0].Keyboard.Enter("C");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            wnds.Find(w => w.Name.StartsWith("Результаты расчета")).Get<Button>().Click();
            wnds = app.GetWindows();
            
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Deselect and Select ALL again
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            Assert.IsTrue(wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnPrint")).Enabled);
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnPrint")).Click();
            wnds = app.GetWindows();
            if (wnds.Find(w => w.Name.StartsWith("Внимание"))!=null)
                wnds.Find(w => w.Name.StartsWith("Внимание")).Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(p => p.Name.StartsWith("Предварительный просмотр")));
        }

        [TestMethod]
        [Description("CA-03-09")]
        public void ExportCalculationResults()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            //Add 2 new analyses
            wnds[0].Mouse.Click(pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses"))
                .GetElement(SearchCriteria.ByAutomationId("IncreaseCommand")).GetClickablePoint());
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Select ALL
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);

            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);//Calculate
            wnds[0].Keyboard.Enter("C");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            wnds.Find(w => w.Name.StartsWith("Результаты расчета")).Get<Button>().Click();
            wnds = app.GetWindows();

            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Deselect and Select ALL again
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            Assert.IsTrue(wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnPrint")).Enabled);
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnPrint")).Click();
            wnds = app.GetWindows();
            if (wnds.Find(w => w.Name.StartsWith("Внимание")) != null)
                wnds.Find(w => w.Name.StartsWith("Внимание")).Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(p => p.Name.StartsWith("Предварительный просмотр")));
            wnds.Find(p => p.Name.StartsWith("Предварительный просмотр")).RightClick();
            pop = wnds.Find(p => p.Name.StartsWith("Предварительный просмотр")).Popup;
            pop.ItemBy(SearchCriteria.ByText("Сохранить как…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(p => p.Name.StartsWith("Сохранение")));
            wnds.Find(p => p.Name.StartsWith("Сохранение")).Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);
        }

        [TestMethod]
        [Description("CA-03-10")]
        public void EvaluateCalculationResults()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")));
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => true), "No Samples");
            string litext = li.Text;
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            //Add new analysis
            pop.ItemBy(SearchCriteria.ByAutomationId("miAddAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);//Select ALL
            wnds[0].Keyboard.Enter("A");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);

            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);//Calculate
            wnds[0].Keyboard.Enter("C");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            wnds.Find(w => w.Name.StartsWith("Результаты расчета")).Get<Button>().Click();
            wnds = app.GetWindows();

            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows[0].Click();
            CADataGridDetails details;
            Assert.IsNotNull(details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter")),
                "Cannot find row details");
            Assert.IsTrue(details.IsLabelVisible(SearchCriteria.ByAutomationId("lblIonicForm")));
            Assert.IsTrue(details.IsLabelVisible(SearchCriteria.ByAutomationId("lblSaltForm")));
        }
        #endregion Analyses
        #region Calibrations
        [TestMethod]
        [Description("CA-04-01")]
        public void CalibrationsList()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByAutomationId("miMainOptions"))
                .SubMenu(SearchCriteria.ByAutomationId("miMainCalibrations"))
                .SubMenu(SearchCriteria.ByAutomationId("miMainCalibrationsKalium")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки")), "List of calibrations unavailable!");
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.ESCAPE);
        }

        [TestMethod]
        [Description("CA-04-02")]
        [Ignore("Not ready yet. Problems with DataGrid")]
        public void CreateCalibrations()
        {
            decimal[,,] calibrationdata = 
                new decimal[,,] { { { 0.001M, 5M }, { 0.0015M, 10M } }, { { 0.01M, 12M }, { 0.02M, 17M } } };

            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByAutomationId("miMainOptions"))
                .SubMenu(SearchCriteria.ByAutomationId("miMainCalibrations"))
                .SubMenu(SearchCriteria.ByAutomationId("miMainCalibrationsKalium")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки")), "List of calibrations unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnNewCalibration")).Click();
            wnds = app.GetWindows();
            Window window = null;
            Assert.IsNotNull(window = wnds.Find(x => x.Name.StartsWith("Данные калибровки")), 
                "Data of the new calibrations unavailable!");
            string calibrationDescription = RandomString.GetRandomString();
            window.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).BulkText = calibrationDescription;
            ListView[] list = new ListView[2];
            Assert.IsNotNull(list[0] = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdFirstDiapason")),
                "No datagrid for 1st diapason");
            Assert.IsNotNull(list[1] = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSecondDiapason")),
                "No datagrid for 2nd diapason");
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    list[j].Rows.FindLast(p => true).DoubleClick();
                    list[j].Rows[i].Cells[0].DoubleClick();
                    window.Enter(calibrationdata[j, i, 0].ToString());
                    window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
                    window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
                    window.Enter(calibrationdata[j, i, 1].ToString());

                }
            }
        }
        #endregion Calibrations
        #region Options
        #endregion Options
    }

    public static class RandomString
    {
        public static string GetRandomString()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[8];
            Random random = new Random();
            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];
            return new String(stringChars);
        }
    }
    [ControlTypeMapping(CustomUIItemType.Custom, WindowsFramework.Wpf)]
    public class CADataGridDetails : CustomUIItem
    {
        public CADataGridDetails(AutomationElement automationElement, IActionListener actionListener)
            : base(automationElement, actionListener) {}
        protected CADataGridDetails() { }
        public string GetScheme()
        {
            return Container.Get<ComboBox>().SelectedItemText;
        }
        public void SetScheme(string schemeName)
        {
            Container.Get<ComboBox>().Select(schemeName);
        }
        public ToggleState GetCheckBoxState()
        {
            return Container.Get<CheckBox>().State;
        }
        public bool IsLabelVisible(SearchCriteria sc)
        {
            return Container.Get<WPFLabel>(sc).Visible;
        }
    }
}