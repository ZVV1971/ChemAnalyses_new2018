using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using TestStack.White;
using TestStack.White.UIItems;
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
        //user with read write rights
        string testuser2 = "catest1";
        string testpwd1 = "ca123_CA123";

        static Application app;

        [TestInitialize]
        public void UITestsInit()
        {
            var appPath = @"e:\IIT\Projects\СВПП\KSR\ChemicalAnalyses\ChemicalAnalyses\bin\Debug\ChemicalAnalyses.exe";
            app = Application.Launch(appPath);
        }

        [TestCleanup]
        public void UITestcleanup()
        {
            app?.Close();
        }

        [TestMethod]
        [TestCategory("Authorization")]
        [Description("CA-01-001")]
        public void AuthorizationPositive()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")),"Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            //Too short to pass the rule
            tbUserName.Enter(new string('a',10));
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(new string('g',10));
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
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
                Window window = app.GetWindow("Авторизация");
                TextBox tbUserName = window.Get<TextBox>("tbUserName");
                TextBox tbPwd = window.Get<TextBox>("pbPassword");
                tbUserName.Enter(new string('a', 10));
                tbPwd.Enter(new string('g', 10));
                window.WaitWhileBusy();
                Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
                btn.Click();
                app.WaitWhileBusy();
                var wnds = app.GetWindows();
                if (i != 3)
                {
                    Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Wrong user came in!");
                    Button btnYes = wnds[0].Get<Button>(SearchCriteria.ByText("Да"));
                    btnYes.Click();
                }
                else
                {
                    Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Ошибка авторизации")), "Wrong user came in!");
                    Button btnYes = wnds[0].Get<Button>(SearchCriteria.ByAutomationId("2"));
                    btnYes.Click();
                }
            }
        }

        [TestMethod]
        [Description("CA-02-003")]
        public void HelpMenu()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Помощь")).SubMenu(SearchCriteria.ByText("О программе…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("О программе…")), "About is not working!");
        }

        [TestMethod]
        [Description("CA-02-004")]
        public void HelpMenuF1()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F1);
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("О программе…")), "About is not working!");
        }

        [TestMethod]
        [Description("CA-02-010")]
        public void SamplesListMenu()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
        }

        [TestMethod]
        [Description("CA-02-010")]
        public void SamplesListMenuByHotKey()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Калибровки")).SubMenu(SearchCriteria.ByText("Калий")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки для:")), "Calibration selection dialog is unavailable!");
        }

        [TestMethod]
        [Description("CA-02-007")]
        public void NaCalibrationMenu()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Калибровки")).SubMenu(SearchCriteria.ByText("Натрий")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Выбор калибровки для:")), "Calibration selection dialog is unavailable!");
        }

        [TestMethod]
        [Description("CA-02-005")]
        public void InitialOptionsMenu()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Настройки")).SubMenu(SearchCriteria.ByText("Исходные данные…")).SubMenu(SearchCriteria.ByText("для анализа солей")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Исходные настройки")), "Initial options dialog is not available!");
        }

        [TestMethod]
        [Description("CA-02-008")]
        public void ChangeUserMenu()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Файл")).SubMenu(SearchCriteria.ByText("Сменить пользователя")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Авторизация")), "Change user failed!");
        }

        [TestMethod]
        [Description("CA-02-009")]
        public void ChangeUserMenuByHotKey()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Файл")).SubMenu(SearchCriteria.ByText("Выход")).Click();
            Assert.IsTrue(app.HasExited, "Exit failed!");
        }

        [TestMethod]
        [Description("CA-02-002")]
        public void ExitMenuByALT_F4()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Keyboard.HoldKey(KeyboardInput.SpecialKeys.ALT);
            wnds[0].Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F4);
            wnds[0].Keyboard.LeaveKey(KeyboardInput.SpecialKeys.ALT);
            Thread.Sleep(1000);// Wait till hot key combination is swallowed by WPF monster
            Assert.IsTrue(app.HasExited, "Exit failed!");
        }

        [TestMethod]
        [Description("CA-03-006")]
        public void SamplesListLoadSamplesFails_UnAuthorizedUser()
        {
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser1);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser2);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
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
            Window window = app.GetWindow("Авторизация");
            TextBox tbUserName = window.Get<TextBox>("tbUserName");
            tbUserName.SetValue(testuser2);
            TextBox tbPwd = window.Get<TextBox>("pbPassword");
            tbPwd.Enter(testpwd1);
            Button btn = window.Get<Button>(SearchCriteria.ByText("OK"));
            btn.Click();
            app.WaitWhileBusy();
            var wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Расчет")), "Вход не осуществлен!");
            wnds[0].Get<Menu>(SearchCriteria.ByText("Образец")).SubMenu(SearchCriteria.ByText("Список…")).Click();
            wnds = app.GetWindows();
            Assert.IsNotNull(wnds.Find(x => x.Name.StartsWith("Список")), "List of samples unavailable!");
            Thread.Sleep(1000);
            wnds[0].Get<Button>(SearchCriteria.ByText("Загрузить список")).Click();
            wnds = app.GetWindows();
            Assert.IsNull(wnds.Find(x => x.Name.StartsWith("Ошибка")), "Data have been seen by an unauthorized user!");
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
}