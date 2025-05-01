using System.Windows.Input;
using System.Windows;

namespace EDH.Presentation.Common.Resources.Behaviors;

public static class WindowBehavior
{

	public static readonly DependencyProperty IsDraggableProperty =
		DependencyProperty.RegisterAttached(
			"IsDraggable",
			typeof(bool),
			typeof(WindowBehavior),
			new PropertyMetadata(false, OnIsDraggableChanged));

	public static bool GetIsDraggable(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsDraggableProperty);
	}

	public static void SetIsDraggable(DependencyObject obj, bool value)
	{
		obj.SetValue(IsDraggableProperty, value);
	}

	private static void OnIsDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is UIElement element)
		{
			if ((bool)e.NewValue)
			{
				element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
			}
			else
			{
				element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
			}
		}
	}

	private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (sender is UIElement element && e.ButtonState == MouseButtonState.Pressed)
		{
			Window.GetWindow(element)?.DragMove();
		}
	}
}