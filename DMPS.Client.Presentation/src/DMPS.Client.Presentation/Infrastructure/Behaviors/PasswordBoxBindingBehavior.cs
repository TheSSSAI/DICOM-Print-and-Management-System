using Microsoft.Xaml.Behaviors;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace DMPS.Client.Presentation.Infrastructure.Behaviors
{
    /// <summary>
    /// A behavior that allows binding to the Password property of a PasswordBox in an MVVM-friendly way.
    /// This is necessary because the Password property is not a dependency property for security reasons.
    /// USAGE:
    /// <PasswordBox>
    ///   <i:Interaction.Behaviors>
    ///     <behaviors:PasswordBoxBindingBehavior Password="{Binding Password, Mode=TwoWay}" />
    ///   </i:Interaction.Behaviors>
    /// </PasswordBox>
    /// </summary>
    public sealed class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        private bool _isUpdating;

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password), typeof(string), typeof(PasswordBoxBindingBehavior),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        /// <summary>
        /// Gets or sets the password string that can be bound to a ViewModel property.
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PasswordChanged += OnPasswordBoxPasswordChanged;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PasswordChanged -= OnPasswordBoxPasswordChanged;
            }
            base.OnDetaching();
        }

        private void OnPasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            _isUpdating = true;
            Password = AssociatedObject.Password;
            _isUpdating = false;
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBoxBindingBehavior behavior && behavior.AssociatedObject != null && !behavior._isUpdating)
            {
                var passwordBox = behavior.AssociatedObject;
                passwordBox.Password = e.NewValue as string;
            }
        }
    }
}