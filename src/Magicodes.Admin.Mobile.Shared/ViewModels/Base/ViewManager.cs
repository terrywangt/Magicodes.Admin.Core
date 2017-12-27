using System;
using System.Globalization;
using System.Reflection;
using Magicodes.Admin.Core;
using Magicodes.Admin.Core.Dependency;
using Xamarin.Forms;

namespace Magicodes.Admin.ViewModels.Base
{
    public static class ViewManager
    {
        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached(
                "AutoWireViewModel",
                typeof(bool),
                typeof(ApplicationBootstrapper),
                default(bool),
                propertyChanged: OnAutoWireViewModelChanged
            );

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Element view))
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }

            if (ApplicationBootstrapper.AbpBootstrapper?.IocManager == null)
            {
                view.BindingContext = Activator.CreateInstance(viewModelType, true);
                return;
            }

            var viewModel = DependencyResolver.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}