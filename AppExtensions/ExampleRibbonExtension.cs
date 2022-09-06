using Addon.Ribbon.Tabs;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

namespace Addon.AppExtensions
{
    public class ExampleRibbonExtension : IExtensionApplication
    {
        #region IExtensionApplication

        public void Initialize()
        {
            /* ленту грузим с помощью обработчика событий:
             * Этот вариант нужно использовать, если ваш плагин
             * стоит в автозагрузке, т.к. он (плагин) инициализируется
             * до построения ленты
             */
            ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;

            // Т.к. мы грузим плагин через NETLOAD, то строим вкладку в ленте сразу
            //BuildRibbonTab();
        }

        /// <summary>
        /// Обработчик события. Следит за событиями изменения окна автокада.
        /// Используем его для того, чтобы "поймать" момент построения ленты,
        /// учитывая, что наш плагин уже инициализировался.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            // Проверяем, что лента загружена
            if (ComponentManager.Ribbon != null)
            {
                // Строим нашу вкладку
                BuildRibbonTab();

                //и раз уж лента запустилась, то отключаем обработчик событий
                ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
            }
        }

        public void Terminate()
        {
        }

        #endregion

        private void BuildRibbonTab()
        {
            var ribbon = ComponentManager.Ribbon;
            var editor = Application.DocumentManager.MdiActiveDocument.Editor;

            editor.WriteMessage($"MyAddon loading...\n");

            if (ribbon.FindTab(MyRibbonTab.RibbonTabId) != null) return;

            try
            {
                ribbon.Tabs.Add(new MyRibbonTab());
                ribbon.UpdateLayout();

                editor.WriteMessage($"MyAddon Loaded.\n");
            }
            catch (System.Exception ex)
            {
                editor.WriteMessage(ex.ToString());
            }
        }
    }
}
