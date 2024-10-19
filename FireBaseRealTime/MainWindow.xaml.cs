using FireBaseRealTime.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FireBaseRealTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _dragStartPoint;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ViewModel();
        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }


        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = _dragStartPoint - mousePos;

            // Kiểm tra nếu chúng ta thực sự đang kéo
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Lấy phần tử ListViewItem
                ListView? listView = sender as ListView;
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Kiểm tra xem có đang kéo từ một phần tử con không (như CheckBox)
                if (listViewItem == null || e.OriginalSource is CheckBox)
                {
                    return;
                }

                // Lấy dữ liệu đằng sau ListViewItem
                AccountInforModel item = (AccountInforModel)listView!.ItemContainerGenerator.ItemFromContainer(listViewItem);

                if (item != null)
                {
                    // Bắt đầu thao tác kéo-thả
                    DataObject dragData = new DataObject("myFormat", item);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                var data = e.Data.GetData("myFormat") as AccountInforModel;
                var listView = sender as ListView;

                // Tìm ListViewItem tổ tiên để lấy targetItem
                var targetItemContainer = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (targetItemContainer != null)
                {
                    var targetItem = targetItemContainer.DataContext as AccountInforModel;

                    if (data != null && targetItem != null && data != targetItem)
                    {
                        var viewModel = DataContext as ViewModel;
                        int oldIndex = viewModel!.ListAccountInforModels.IndexOf(data);
                        int newIndex = viewModel!.ListAccountInforModels.IndexOf(targetItem);

                        if (oldIndex != -1 && newIndex != -1 && oldIndex != newIndex)
                        {
                            viewModel.ListAccountInforModels.Move(oldIndex, newIndex);
                            foreach (var item in viewModel.ListAccountInforModels)
                            {
                                item.NoId = viewModel.ListAccountInforModels.IndexOf(item) + 1;
                            }
                        }
                    }
                }
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void ListAcc_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }

}