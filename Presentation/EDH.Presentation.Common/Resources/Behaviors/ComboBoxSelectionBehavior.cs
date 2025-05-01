using System.Windows;
using System.Windows.Controls;

namespace EDH.Presentation.Common.Resources.Behaviors;

public static class ComboBoxSelectionBehavior
{
	public static readonly DependencyProperty PreserveCaretPositionProperty =
		DependencyProperty.RegisterAttached(
			"PreserveCaretPosition",
			typeof(bool),
			typeof(ComboBoxSelectionBehavior),
			new UIPropertyMetadata(false, OnPreserveCaretPositionChanged));

	public static void SetPreserveCaretPosition(DependencyObject obj, bool value) =>
		obj.SetValue(PreserveCaretPositionProperty, value);

	public static bool GetPreserveCaretPosition(DependencyObject obj) =>
		(bool)obj.GetValue(PreserveCaretPositionProperty);

	private static void OnPreserveCaretPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ComboBox combo) return;

		if ((bool)e.NewValue)
		{
			combo.DropDownOpened += Combo_DropDownOpened;
			combo.SelectionChanged += Combo_SelectionChanged;
			combo.TextInput += Combo_TextInput;
			if (combo.IsEditable)
			{
				combo.Loaded += (sender, args) =>
				{
					if (combo.Template.FindName("PART_EditableTextBox", combo) is TextBox textBox)
					{
						textBox.TextChanged += TextBox_TextChanged;
					}
				};
			}
		}
		else
		{
			combo.DropDownOpened -= Combo_DropDownOpened;
			combo.SelectionChanged -= Combo_SelectionChanged;
			combo.TextInput -= Combo_TextInput;
			if (combo.IsEditable && combo.Template.FindName("PART_EditableTextBox", combo) is TextBox textBox)
			{
				textBox.TextChanged -= TextBox_TextChanged;
			}
		}
	}

	private static void Combo_DropDownOpened(object? sender, EventArgs e)
	{
		PreserveCaretAtEnd(sender as ComboBox);
	}

	private static void Combo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		PreserveCaretAtEnd(sender as ComboBox);
	}

	private static void Combo_TextInput(object? sender, System.Windows.Input.TextCompositionEventArgs e)
	{
	}

	private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is not TextBox textBox) return;

		if (textBox.Text.Length == 0) return;

		int caretIndex = textBox.CaretIndex;

		if (caretIndex == 0)
		{
			int changeOffset = e.Changes.Sum(change => change.AddedLength - change.RemovedLength);
			int newCaretIndex = caretIndex + changeOffset;

			if (newCaretIndex >= 0 && newCaretIndex <= textBox.Text.Length)
			{
				textBox.CaretIndex = newCaretIndex;
			}
		}
	}

	private static void PreserveCaretAtEnd(ComboBox? combo)
	{
		if (combo == null) return;

		if (combo.IsEditable && combo.Template.FindName("PART_EditableTextBox", combo) is TextBox tb && !string.IsNullOrEmpty(tb.Text))
		{
			try
			{
				tb.SelectionStart = tb.Text.Length;
				tb.SelectionLength = 0;
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}
	}
}
