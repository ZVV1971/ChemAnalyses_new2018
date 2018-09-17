using System.Windows.Input;

namespace ChemicalAnalyses.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Выход",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection
                {new KeyGesture (Key.F4,ModifierKeys.Alt)});
        public static readonly RoutedUICommand Options = new RoutedUICommand(
            "Настройки калибровок",
            "Options",
            typeof(CustomCommands),
            new InputGestureCollection
                {new KeyGesture (Key.O,ModifierKeys.Alt)});
        public static readonly RoutedUICommand Edit = new RoutedUICommand(
            "Редактировать",
            "Edit",
            typeof(CustomCommands),
            new InputGestureCollection
                {new KeyGesture (Key.E,ModifierKeys.Control)});

        public static readonly RoutedUICommand Update = new RoutedUICommand(
            "Update",
            "Update",
            typeof(CustomCommands),
            new InputGestureCollection
                {new KeyGesture (Key.U,ModifierKeys.Alt)});

        public static readonly RoutedUICommand Filter = new RoutedUICommand(
            "Filter",
            "Filter",
            typeof(CustomCommands),
            new InputGestureCollection
                {new KeyGesture (Key.F,ModifierKeys.Alt)});
        public static readonly RoutedUICommand List = new RoutedUICommand(
           "List",
           "List",
           typeof(CustomCommands),
           new InputGestureCollection
               {new KeyGesture (Key.L,ModifierKeys.Control)});
        public static readonly RoutedUICommand View = new RoutedUICommand(
           "View",
           "View",
           typeof(CustomCommands),
           new InputGestureCollection
               {new KeyGesture (Key.V,ModifierKeys.Alt)});

        public static readonly RoutedUICommand SetDefault = new RoutedUICommand(
           "SetDefault",
           "SetDefault",
           typeof(CustomCommands),
           new InputGestureCollection
               {new KeyGesture (Key.D,ModifierKeys.Alt)});

        public static readonly RoutedUICommand Calculate = new RoutedUICommand(
          "Calculate",
          "Calculate",
          typeof(CustomCommands),
          new InputGestureCollection
              {new KeyGesture (Key.C,ModifierKeys.Alt)});

        public static readonly RoutedUICommand AddNewAnalysis = new RoutedUICommand(
          "Добавить новый анализ",
          "AddNewAnalysis",
          typeof(CustomCommands));
    }
}