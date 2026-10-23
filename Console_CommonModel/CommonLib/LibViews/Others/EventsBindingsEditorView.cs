using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Reflection;

namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Представление для настройки обработчиков событий")]
    public class EventsBindingsEditorView : AppViewBase
    {
        private AppViewBase _targetView;
        private Dictionary<string, ChangingList<string>> eventsBinds = new();
        private ScrollView scrollView;

        public EventsBindingsEditorView()
        {
            MinWidth = 640;
            MinHeight = 480;
        }

        public void Bind(AppViewBase view)
        {
            _targetView = view;
            eventsBinds = view.EventsBindings;

            foreach (var eventName in view.GetBaseEventProperties())
            {
                if (!eventsBinds.ContainsKey(eventName))
                    eventsBinds[eventName] = new ChangingList<string>();
            }

            UpdateEventsBindingsEditorView();
        }

        public void UpdateEventsBindingsEditorView()
        {
            Controls.Clear();
            LayoutFill(scrollView = new ScrollView());
            if (_targetView is null)
                return;

            var header = CreateText(GetTargetCaption(_targetView));
            header.MinHeight = 32;
            scrollView.LayoutTop(header);

            var applyButton = CreateButton("Применить (скомпилировать)", () =>
            {
                _targetView.CompileEventsBindings();
                this.Info("Обработчики событий скомпилированы");
            });
            applyButton.MinHeight = 40;
            scrollView.LayoutTop(applyButton);

            foreach (var eventName in _targetView.GetBaseEventProperties())
            {
                if (!eventsBinds.TryGetValue(eventName, out var expressions))
                {
                    expressions = new ChangingList<string>();
                    eventsBinds[eventName] = expressions;
                }


                scrollView.LayoutTop(CreateEventCard(eventName, expressions));
            }
        }

        private AppViewBase CreateEventCard(string eventName, ChangingList<string> expressions)
        {
            var card = new AppViewBase
            {
                MinHeight = 360,
                Padding = 8
            };

            var collection = CreateEventExpressionsControl(expressions);
            card.LayoutTop(collection);

            var title = card.CreateText(GetEventDisplayName(eventName));
            title.MinHeight = 28;
            card.LayoutTop(title);
            return card;
        }

        private static CollectionsView CreateEventExpressionsControl(ChangingList<string> expressions)
        {
            var collection = new CollectionsView()
            {
                Dock = "Top"
            };

            foreach (var expression in expressions)
                collection.values.Add(expression);

            collection.OnAdded += (_, p) => expressions.Add($"{p}");
            collection.OnValueChanged += (i, p) => expressions[i] = $"{p}";
            collection.OnRemoved += (i, _) => expressions.RemoveAt(i);

            return collection;
        }

        private string GetEventDisplayName(string propertyName)
        {
            var property = _targetView?.GetType().GetProperty(propertyName);
            var label = property?.GetCustomAttribute<LabelAttribute>()?.Text;
            return string.IsNullOrWhiteSpace(label) ? propertyName : label;
        }

        private static string GetTargetCaption(AppViewBase view)
        {
            var caption = view.GetType().Name;
            var textProperty = view.GetType().GetProperty("Text");
            if (textProperty?.GetValue(view) is string text && !string.IsNullOrWhiteSpace(text))
                caption = $"{caption}: {text}";
            return caption;
        }

        public static void Test()
        {
            var button = new ButtonView();
            var view = new EventsBindingsEditorView();
            view.Bind(button);
            view.RunAtForm();
        }
    }
}
