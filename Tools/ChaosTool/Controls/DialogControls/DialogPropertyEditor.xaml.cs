using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using ChaosTool.Definitions;
using ChaosTool.Extensions;
using ChaosTool.Model;

namespace ChaosTool.Controls.DialogControls;

public sealed partial class DialogPropertyEditor
{
    private Point DragPoint;
    public ListViewItem<DialogTemplateSchema, DialogPropertyEditor> ListItem { get; }
    public ObservableCollection<DialogOptionSchema> OptionsViewItems { get; }
    public ObservableCollection<string> ScriptKeysViewItems { get; }
    public TraceWrapper<DialogTemplateSchema> Wrapper => ListItem.Wrapper;

    public DialogPropertyEditor(ListViewItem<DialogTemplateSchema, DialogPropertyEditor> listItem)
    {
        ListItem = listItem;
        OptionsViewItems = new ObservableCollection<DialogOptionSchema>();
        ScriptKeysViewItems = new ObservableCollection<string>();

        InitializeComponent();
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TypeCmbox.ItemsSource = GetEnumNames<ChaosDialogType>();
        OptionsView.ItemsSource = OptionsViewItems;
        ScriptKeysView.ItemsSource = ScriptKeysViewItems;

        PopulateControlsFromItem();
    }

    #region Controls > Template > Controls
    public void CopySelectionsToItem()
    {
        var template = Wrapper.Object;

        Wrapper.Path = PathTbox.Text;
        template.TemplateKey = TemplateKeyTbox.Text;
        template.Type = ParsePrimitive<ChaosDialogType>(TypeCmbox.Text);
        template.Text = TextTbox.Text.ReplaceLineEndings("\n");
        template.NextDialogKey = NextDialogKeyTbox.Text;
        template.PrevDialogKey = PrevDialogKeyTbox.Text;
        template.Contextual = ContextualCbox.IsChecked ?? false;
        template.TextBoxLength = ParsePrimitive<ushort?>(TextBoxLengthTbox.Text);
        template.Options = OptionsViewItems.Select(ShallowCopy<DialogOptionSchema>.Create).ToList();
        template.ScriptKeys = ScriptKeysViewItems.ToList();

        ListItem.Name = template.TemplateKey;
    }

    public void PopulateControlsFromItem()
    {
        var template = Wrapper.Object;

        PathTbox.Text = Wrapper.Path;
        TemplateKeyTbox.Text = template.TemplateKey;
        TypeCmbox.SelectedItem = SelectPrimitive(template.Type, TypeCmbox.ItemsSource);
        TextTbox.Text = template.Text.ReplaceLineEndings();
        NextDialogKeyTbox.Text = template.NextDialogKey;
        PrevDialogKeyTbox.Text = template.PrevDialogKey;
        ContextualCbox.IsChecked = template.Contextual;
        TextBoxLengthTbox.Text = template.TextBoxLength?.ToString();

        OptionsViewItems.Clear();
        OptionsViewItems.AddRange(template.Options.Select(ShallowCopy<DialogOptionSchema>.Create));

        ScriptKeysViewItems.Clear();
        ScriptKeysViewItems.AddRange(template.ScriptKeys.ToList());
    }
    #endregion

    #region Buttons
    private void RevertBtn_Click(object sender, RoutedEventArgs e) => PopulateControlsFromItem();

    private async void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var existing = JsonContext.DialogTemplates.Objects.Where(obj => obj != Wrapper)
                                      .FirstOrDefault(obj => obj.Path == Wrapper.Path);

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue($"Save failed. An item already exists at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.DialogTemplates.Objects.Where(obj => obj != Wrapper)
                                  .FirstOrDefault(obj => obj.Object.TemplateKey.EqualsI(Wrapper.Object.TemplateKey));

            if (existing is not null)
            {
                Snackbar.MessageQueue?.Enqueue(
                    $"Save failed. An item already exists with template key \"{existing.Object.TemplateKey}\" at path \"{existing.Path}\"");

                return;
            }

            existing = JsonContext.DialogTemplates.Objects.FirstOrDefault(obj => ReferenceEquals(obj, Wrapper));

            if (existing is null)
                JsonContext.DialogTemplates.Objects.Add(Wrapper);

            if (!ValidatePreSave(Wrapper, PathTbox, TemplateKeyTbox))
            {
                Snackbar.MessageQueue?.Enqueue("Filename does not match the template key");

                return;
            }

            CopySelectionsToItem();
            PopulateControlsFromItem();
        } catch (Exception ex)
        {
            Snackbar.MessageQueue?.Enqueue(ex.ToString());
        }

        await JsonContext.DialogTemplates.SaveItemAsync(Wrapper);
    }
    #endregion

    #region Tbox Validation
    private void TboxNumberValidator(object sender, TextCompositionEventArgs e) => Validators.NumberValidationTextBox(sender, e);
    private void TemplateKeyTbox_OnKeyUp(object sender, KeyEventArgs e) => Validators.TemplateKeyMatchesFileName(TemplateKeyTbox, PathTbox);
    #endregion

    #region DialogOptions Controls
    private void DeleteDialogOptionBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not DialogOptionSchema schema)
            return;

        OptionsViewItems.Remove(schema);
    }

    private void AddDialogOptionBtn_Click(object sender, RoutedEventArgs e)
    {
        var options = new DialogOptionSchema();

        OptionsViewItems.Add(options);
    }
    #endregion

    #region ScriptKeys Controls
    private void AddScriptKeyBtn_Click(object sender, RoutedEventArgs e) => ScriptKeysViewItems.Add(string.Empty);

    private void DeleteScriptKeyBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not string scriptKey)
            return;

        ScriptKeysViewItems.Remove(scriptKey);
    }
    #endregion

    #region Drag Reorder
    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        var point = e.GetPosition(null);
        var diff = DragPoint - point;

        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        if (sender is not ListView)
            return;

        if ((Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance)
            && (Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance))
            return;

        var parent = ((DependencyObject)e.OriginalSource).FindVisualParent<ListViewItem>();

        if (parent is null)
            return;

        DragDrop.DoDragDrop(parent, parent.DataContext, DragDropEffects.Move);
    }

    private void ListViewItem_Drop(object sender, DragEventArgs e)
    {
        if (sender is not ListViewItem viewItem)
        {
            viewItem = ((DependencyObject)sender).FindVisualParent<ListViewItem>()!;

            if (viewItem is null)
                return;
        }

        if (viewItem.DataContext is not DialogOptionSchema target)
            return;

        if (e.Data.GetData(typeof(DialogOptionSchema)) is not DialogOptionSchema dropped)
            return;

        var removedId = OptionsView.Items.IndexOf(dropped);
        var targetId = OptionsView.Items.IndexOf(target);

        if (removedId < targetId)
        {
            OptionsViewItems.Insert(targetId + 1, dropped);
            OptionsViewItems.RemoveAt(removedId);
        } else
        {
            removedId++;

            if (OptionsViewItems.Count + 1 > removedId)
            {
                OptionsViewItems.Insert(targetId, dropped);
                OptionsViewItems.RemoveAt(removedId);
            }
        }
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragPoint = e.GetPosition(null);
    #endregion
}