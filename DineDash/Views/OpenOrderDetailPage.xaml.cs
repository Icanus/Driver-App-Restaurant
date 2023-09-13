using DineDash.Models;
using DineDash.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DineDash.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpenOrderDetailPage : ContentPage
    {
        private bool isAnimating = false;
        private Rectangle currentLayoutBounds;

        OrderDetailViewModel viewModel;
        public string OrderId { get; set; }
        public OpenOrderParameter Order { get; set; }
        public OpenOrderDetailPage(bool IsAcceptOrderView = true)
        {
            InitializeComponent();
            viewModel = new OrderDetailViewModel(IsAcceptOrderView);
            BindingContext = viewModel;
            currentLayoutBounds = new Rectangle(0.5, 1, 1, 0.6);
            UpdateStackLayoutLayoutBounds();
        }

        private async void OnSwipeUp(object sender, SwipedEventArgs e)
        {
            await AnimateLayoutChange(new Rectangle(0.5, 1, 1, 0.6));
        }

        private async void OnSwipeDown(object sender, SwipedEventArgs e)
        {
            await AnimateLayoutChange(new Rectangle(0.5, 1, 1, 0.2));
        }

        private async Task AnimateLayoutChange(Rectangle newLayoutBounds)
        {
            if (isAnimating)
                return;

            isAnimating = true;

            var animationDuration = 250; // Adjust the animation duration as needed
            var easing = Easing.CubicInOut; // Choose an easing function

            var startBounds = currentLayoutBounds; // Store the starting layout bounds
            var animation = new Animation(v =>
            {
                currentLayoutBounds = new Rectangle(
                    startBounds.X + (newLayoutBounds.X - startBounds.X) * v,
                    startBounds.Y + (newLayoutBounds.Y - startBounds.Y) * v,
                    startBounds.Width + (newLayoutBounds.Width - startBounds.Width) * v,
                    startBounds.Height + (newLayoutBounds.Height - startBounds.Height) * v);

                UpdateStackLayoutLayoutBounds();
            });

            animation.Commit(this, "LayoutAnimation", length: (uint)animationDuration, easing: easing);

            await Task.Delay(animationDuration);
            isAnimating = false;
        }

        private void UpdateStackLayoutLayoutBounds()
        {
            AbsoluteLayout.SetLayoutBounds(stackLayout, currentLayoutBounds);
        }

        #region hide
        /*
        private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    initialY = e.TotalY;
                    initialGridHeight = draggableGrid.Height;
                    originalSnapHeight = DeviceDisplay.MainDisplayInfo.Height / 8;
                    isDragging = true;
                    isDraggingDown = false;
                    break;

                case GestureStatus.Running:
                    if (isDragging)
                    {
                        if (e.TotalY > initialY)
                        {
                            isDraggingDown = true;
                        }
                        else
                        {
                            isDraggingDown = false;
                        }

                        double delta = e.TotalY - initialY;
                        double newHeight = initialGridHeight + (isDraggingDown ? delta : -delta);

                        // Limit maximum drag up height to DeviceDisplay.MainDisplayInfo.Height / 8
                        if (!isDraggingDown && newHeight > (DeviceDisplay.MainDisplayInfo.Height / 8))
                        {
                            newHeight = DeviceDisplay.MainDisplayInfo.Height / 12;
                        }

                        newHeight = Math.Max(Math.Min(newHeight, originalSnapHeight), 100); // Clamp within range

                        // Update the grid height
                        UpdateGridHeight(newHeight);

                        // Translate the grid layout to simulate drag movement
                        await draggableGrid.TranslateTo(draggableGrid.TranslationX, delta, 200); // Adjust speed if needed
                    }
                    break;

                case GestureStatus.Canceled:
                case GestureStatus.Completed:
                    if (isDragging)
                    {
                        isDragging = false;

                        double targetHeight = isDraggingDown ? 100 : originalSnapHeight;

                        // Snap the grid height and reset the translation
                        UpdateGridHeight(targetHeight);
                        await draggableGrid.TranslateTo(draggableGrid.TranslationX, 0, 250); // Animate back to original position
                    }
                    break;
            }
        }

        private void UpdateGridHeight(double height)
        {
            draggableGrid.HeightRequest = height;
        }
        */
        #endregion
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Order?.OrderId != null)
            {
                viewModel.OpenOrder = Order;
            }
        }
    }
}