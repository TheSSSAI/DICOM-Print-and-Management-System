using Microsoft.Xaml.Behaviors;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace DMPS.Client.Presentation.Infrastructure.Behaviors
{
    /// <summary>
    /// A custom behavior to enable secure, two-way data binding for the Password property of a PasswordBox.
    /// This works around a known WPF limitation where PasswordBox.Password is not a dependency property for security reasons.
    /// Usage:
    /// <PasswordBox>
    ///     <i:Interaction.Behaviors>
    ///         <behaviors:PasswordBoxBindingBehavior Password="{Binding Password, Mode=TwoWay}" />
    ///     </i:Interaction.Behaviors>
    /// </PasswordBox>
    /// </summary>
    public sealed class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        private bool _isUpdating;

        /// <summary>
        /// DependencyProperty for the Password property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(nameof(Password), typeof(string), typeof(PasswordBoxBindingBehavior),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordPropertyChanged));

        /// <summary>
        /// Gets or sets the password string from the ViewModel.
        /// </summary>
        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBoxBindingBehavior behavior && behavior.AssociatedObject != null)
            {
                var passwordBox = behavior.AssociatedObject;
                if (behavior._isUpdating)
                {
                    return;
                }

                passwordBox.Password = (string)e.NewValue;
            }
        }
        
        /// <summary>
        /// Called when the behavior is attached to the PasswordBox.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.PasswordChanged += OnPasswordBoxPasswordChanged;
                // Set initial value
                AssociatedObject.Password = Password;
            }
        }

        /// <summary>
        /// Called when the behavior is detaching from the PasswordBox.
        /// </summary>
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
            if (sender is not PasswordBox passwordBox)
            {
                return;
            }
            
            _isUpdating = true;
            try
            {
                Password = passwordBox.Password;
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}