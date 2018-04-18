using System.Windows.Input;

namespace ChemicalAnalyses.Commands
{
    public static class CustomKSRCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Выход",
            "Exit",
            typeof(CustomKSRCommands),
            new InputGestureCollection
                {
                    new KeyGesture (Key.F4,ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand Options = new RoutedUICommand(
            "Настройки калибровок",
            "Options",
            typeof(CustomKSRCommands),
            new InputGestureCollection
                {
                    new KeyGesture (Key.O,ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand Edit = new RoutedUICommand(
            "Редактировать",
            "Edit",
            typeof(CustomKSRCommands),
            new InputGestureCollection
                {
                    new KeyGesture (Key.E,ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Update = new RoutedUICommand(
            "Update",
            "Update",
            typeof(CustomKSRCommands),
            new InputGestureCollection
                {
                    new KeyGesture (Key.U,ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand Filter = new RoutedUICommand(
            "Filter",
            "Filter",
            typeof(CustomKSRCommands),
            new InputGestureCollection
                {
                    new KeyGesture (Key.F,ModifierKeys.Alt)
                }
            );
        public static readonly RoutedUICommand List = new RoutedUICommand(
           "List",
           "List",
           typeof(CustomKSRCommands),
           new InputGestureCollection
               {
                    new KeyGesture (Key.L,ModifierKeys.Control)
               }
           );
        public static readonly RoutedUICommand View = new RoutedUICommand(
           "View",
           "View",
           typeof(CustomKSRCommands),
           new InputGestureCollection
               {
                    new KeyGesture (Key.V,ModifierKeys.Alt)
               }
           );

        public static readonly RoutedUICommand SetDefault = new RoutedUICommand(
           "SetDefault",
           "SetDefault",
           typeof(CustomKSRCommands),
           new InputGestureCollection
               {
                    new KeyGesture (Key.D,ModifierKeys.Alt)
               }
           );

        public static readonly RoutedUICommand Calculate = new RoutedUICommand(
          "Calculate",
          "Calculate",
          typeof(CustomKSRCommands),
          new InputGestureCollection
              {
                    new KeyGesture (Key.C,ModifierKeys.Alt)
              }
          );
    }
}