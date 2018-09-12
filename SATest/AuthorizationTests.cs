using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Windows.Automation;

using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.MenuItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.Custom;
using TestStack.White.UIItems.Actions;
using TestStack.White.Factory;
using TestStack.White.Configuration;
using TestStack.White.WindowsAPI;

namespace SATest
{
    [TestClass]
    public class UITests
    {
        //dummy user just to get into system, has no rights whatever
        string testuser1 = "catest";
        //user with read write rights
        string testuser2 = "catest1";
        string testpwd1 = "ca123_CA123";

        static Application app;

        [TestInitialize]
        public void UITestsInit()
        {
            CoreAppXmlConfiguration.Instance.BusyTimeout = 3000;
            var appPath = @"e:\IIT\Projects\СВПП\KSR\ChemicalAnalyses\ChemicalAnalyses\bin\Debug\ChemicalAnalyses.exe";
            app = Application.Launch(appPath);
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

        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-001")]
        public void AuthorizationPositive()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
        }

        [TestMethod]
        [TestCategory("Authorization")]
        public void AuthorizationNegativeUserNameTooShort()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            //Too short to pass the rule
            tbUserName.SetValue("kk");
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(" ");
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            Assert.IsFalse(btn.Enabled);
        }

        [TestMethod]
        [TestCategory("Authorization")]
        public void AuthorizationNegativePwdTooShort()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            //Too short to pass the rule
            tbUserName.SetValue("kkf");
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(" ");
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            Assert.IsFalse(btn.Enabled);
        }

        [TestMethod]
        [TestCategory("Authorization")]
        public void AuthorizationNegativePwdTooShortDescPresent()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            //Too short to pass the rule
            tbUserName.Enter("kkf");
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(" ");
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            Assert.IsFalse(btn.Enabled);
            Label lbl = (Label)window.Get(SearchCriteria.ByText("Пароль от 3 символов!"));
            Assert.IsTrue(lbl.Visible);
        }

        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-002")]
        public void AuthorizationNegativeWrongUser()
        {
            Assert.IsFalse(Authorize(new string('a', 10), new string('g', 10)), "Wrong user came in!");
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Wrong user came in!");
        }

        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-003")]
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

        [TestMethod]
        [Description("CA-02-003")]
        public void HelpMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Помощь")).SubMenu(SearchCriteria.ByText("О программе…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("О программе…")), "About is not working!");
        }

        [TestMethod]
        [Description("CA-02-004")]
        public void HelpMenuF1()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F1);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("О программе…")), "About is not working!");
        }

        [TestMethod]
        [Description("CA-02-010")]
        public void SamplesListMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
        }

        [TestMethod]
        [Description("CA-02-010")]
        public void SamplesListMenuByHotKey()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
            wnds[0].Keyboard.Enter("L");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
        }

        [TestMethod]
        [Description("CA-02-006")]
        public void KCalibrationMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Калибровки")).SubMenu(SearchCriteria.ByText("Калий")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки для:")), "Calibration selection dialog is unavailable!");
        }

        [TestMethod]
        [Description("CA-02-007")]
        public void NaCalibrationMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Калибровки")).SubMenu(SearchCriteria.ByText("Натрий")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки для:")), "Calibration selection dialog is unavailable!");
        }

        [TestMethod]
        [Description("CA-02-005")]
        public void InitialOptionsMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Исходные данные…")).SubMenu(SearchCriteria.ByText("для анализа солей")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Исходные настройки")), "Initial options dialog is not available!");
        }

        [TestMethod]
        [Description("CA-02-008")]
        public void ChangeUserMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Файл")).SubMenu(SearchCriteria.ByText("Сменить пользователя")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Авторизация")), "Change user failed!");
        }

        [TestMethod]
        [Description("CA-02-009")]
        public void ChangeUserMenuByHotKey()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);
            wnds[0].Keyboard.Enter("R");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Авторизация")), "Change user failed!");
        }

        [TestMethod]
        [Description("CA-02-001")]
        public void ExitMenu()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Файл")).SubMenu(SearchCriteria.ByText("Выход")).Click();
            app.WaitWhileBusy();
            Assert.IsTrue(app.HasExited, "Exit failed!");
        }

        [TestMethod]
        [Description("CA-02-002")]
        public void ExitMenuByALT_F4()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F4);
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            Thread.Sleep(1500);// Wait till hot key combination is swallowed by WPF monster
            Assert.IsTrue(app.HasExited, "Exit failed!");
        }

        [TestMethod]
        [Description("CA-03-006")]
        public void SamplesListLoadSamplesFails_UnAuthorizedUser()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
        }

        [TestMethod]
        [Description("CA-03-001")]
        public void SamplesListLoadSamplesAddSample()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Добавить…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Внести новый образец")), "Adding unavailable!");
        }

        [TestMethod]
        [Description("CA-03-002")]
        public void SamplesListLoadSamplesAddSampleByCTRL_N()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
            wnds[0].Keyboard.Enter("N");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Внести новый образец")), "Adding unavailable!");
        }

        [TestMethod]
        [Description("CA-03-003")]
        public void SamplesListLoadSamplesFilter()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Фильтр…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Отбор образцов по параметрам")), "Filter unavailable!");
        }

        [TestMethod]
        [Description("CA-03-002")]
        public void SamplesListLoadSamplesFilterByALT_F()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);
            wnds[0].Keyboard.Enter("F");
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Отбор образцов по параметрам")), "Filter unavailable!");
        }

        [TestMethod]
        [Description("CA-03-005")]
        public void SamplesListLoadSamplesExit()
        {
            Assert.IsTrue(Authorize(testuser1, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Выход")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples don't exit!");
        }

        [TestMethod]
        [Description("CA-03-007")]
        public void SamplesListLoadSamplesPositive()
        {
            Assert.IsTrue(Authorize(testuser2, testpwd1), "Вход не осуществлен!");
            var wnds = app.GetWindows();
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.AreNotEqual(lb.Items.Count, 0);
        }

        [TestMethod]
        [Description("CA-03-008")]
        public void SamplesListAddDeleteSample()
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
            wnds[0].Get<Button>(SearchCriteria.ByText("Добавить…")).Click();
            wnds = app.GetWindows();
            Window wnd;
            Assert.IsNotNull(wnd = wnds.Find(x => x.Name.StartsWith("Внести новый образец")), "Adding unavailable!");
            string labNumber = RandomString.GetRandomString();
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbLabNumber")).SetValue(labNumber);
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).Enter("No description");
            wnd.Get<Button>(SearchCriteria.ByText("OK")).Click();
            Thread.Sleep(1000);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text.Contains(labNumber)), "Sample wasn't added");
            lb.Select(li.Text);
            wnds[0].Get<Button>(SearchCriteria.ByText("Удалить")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "Deletion of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            Assert.IsNull(li = lb.Items.Find(x => x.Text.Contains(labNumber)), "Sample wasn't deleted");
        }

        [TestMethod]
        [Description("CA-03-009")]
        public void SamplesListAddDeleteAnalysisToSample()
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
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
            li.RightClick();
            pop = wnds[0].Popup;
            Menu mn;
            Assert.IsNotNull(mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")));
            Assert.IsTrue(mn.Enabled);
            mn.Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows[0].Select();
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnDelete")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "No deletion warning!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            wnds[0].Get<Button>(SearchCriteria.ByText("Отмена")).Click();
            wnds = app.GetWindows();
            lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
            li.RightClick();
            pop = wnds[0].Popup;
            mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses"));
            Assert.IsFalse(mn.Enabled);
            //Assert.ThrowsException<UIItemSearchException> (ac);//unable to find disabled menu item
        }

        [TestMethod]
        [Description("CA-03-010")]
        public void SamplesListEditSample()
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
            li.Select();
            wnds[0].Get<Button>(SearchCriteria.ByAutomationId("btnEditSample")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактировать")), "Sample editing is not available!");
        }

        [TestMethod]
        [Description("CA-03-011")]
        public void SamplesListEditSampleContextMenu()
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
            //li.Select();
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miEditSample")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактировать")), "Sample editing is not available!");
        }

        [TestMethod]
        [Description("CA-03-012")]
        public void SamplesListAddDeleteSampleContextMenu()
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
            wnds[0].Get<Button>(SearchCriteria.ByText("Добавить…")).Click();
            wnds = app.GetWindows();
            Window wnd;
            Assert.IsNotNull(wnd = wnds.Find(x => x.Name.StartsWith("Внести новый образец")), "Adding unavailable!");
            string labNumber = RandomString.GetRandomString();
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbLabNumber")).SetValue(labNumber);
            wnd.Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).Enter("No description");
            wnd.Get<Button>(SearchCriteria.ByText("OK")).Click();
            Thread.Sleep(1000);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            ListBox lb = wnds[0].Get<ListBox>(SearchCriteria.ByAutomationId("lbSamples"));
            ListItem li;
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text.Contains(labNumber)), "Sample wasn't added");
            lb.Select(li.Text);
            li.RightClick();
            PopUpMenu pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miDeleteSample")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "Deletion of samples unavailable!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            Assert.IsNull(li = lb.Items.Find(x => x.Text.Contains(labNumber)), "Sample wasn't deleted");
        }

        [TestMethod]
        [Description("CA-03-013")]
        public void SamplesListAddDeleteAnalysisToSampleContextMenu()
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
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
            li.RightClick();
            pop = wnds[0].Popup;
            Menu mn;
            Assert.IsNotNull(mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")));
            Assert.IsTrue(mn.Enabled);
            mn.Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            ListView list;
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows[0].Select();
            list.Rows[0].RightClick();
            pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miDeleteAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "No deletion warning!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            wnds[0].Get<Button>(SearchCriteria.ByText("Отмена")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
            li.RightClick();
            pop = wnds[0].Popup;
            mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses"));
            Assert.IsFalse(mn.Enabled);
        }

        [TestMethod]
        [Description("CA-03-014")]
        public void SamplesListAddAnalysisDescription()
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
            list.Rows[0].DoubleClick();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Описание")), "Description of samples unavailable!");
            string sampleDescription = RandomString.GetRandomString();
            wnds[0].Get<TextBox>(SearchCriteria.ByAutomationId("tbDescription")).BulkText = sampleDescription;
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Новые данные")), "Cannot add analyses");
            //CADataGridDetails details = wnds[0].Get<CADataGridDetails>(SearchCriteria.ByClassName("DataGridDetailsPresenter"));
            //System.Windows.Rect rc = details.Bounds;
            //wnds[0].Mouse.Location = new System.Windows.Point((rc.Left + rc.Right) / 2, (rc.Top + rc.Bottom) / 2);
            //Thread.Sleep(3000);
            //var tmp = wnds[0].ToolTip.Text;
            
            //Assert.IsTrue(tmp.Contains(sampleDescription));
            wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
                        
            li.RightClick();
            pop = wnds[0].Popup;
            Menu mn;
            Assert.IsNotNull(mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses")));
            Assert.IsTrue(mn.Enabled);
            mn.Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            Assert.IsNotNull(list = wnds[0].Get<ListView>(SearchCriteria.ByAutomationId("dgrdSA")), "No datagrid");
            list.Rows[0].Select();
            list.Rows[0].RightClick();
            pop = wnds[0].Popup;
            pop.ItemBy(SearchCriteria.ByAutomationId("miDeleteAnalyses")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Удаление")), "No deletion warning!");
            wnds[0].Get<Button>(SearchCriteria.ByText("Да")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Редактирование данных")), "Cannot edit analyses");
            wnds[0].Get<Button>(SearchCriteria.ByText("Отмена")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(li = lb.Items.Find(x => x.Text == litext), "No Samples");
            li.RightClick();
            pop = wnds[0].Popup;
            mn = pop.ItemBy(SearchCriteria.ByAutomationId("miEditAnalyses"));
            Assert.IsFalse(mn.Enabled);
        }

        [TestMethod]
        [Description("CA-03-015")]
        public void SamplesListSetAnalysisScheme()
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
            Assert.AreEqual( details.GetScheme(),"Хлоридная");//defualt scheme
            details.SetScheme("Карбонатная");       //not yet realized scheme
            Assert.IsFalse(wnds[0].Get<Button>(SearchCriteria.ByText("OK")).Enabled);
        }

        [TestMethod]
        [Description("CA-03-016")]
        public void SamplesListCalculateAnalysis()
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
    }
}