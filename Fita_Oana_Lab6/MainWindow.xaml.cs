using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AutoLotModel;

namespace Fita_Oana_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            ctx.Inventories.Load();
            ctx.Orders.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join inv in ctx.Inventories on ord.CarId equals inv.CarId
                             select new
                             {
                                 ord.OrderId, ord.CarId, ord.CustId,
                                 cust.FirstName, cust.LastName,
                                 inv.Make, inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem entitatea Customer
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewCustomer.IsEnabled = true;
                btnEditCustomer.IsEnabled = true;
                btnDeleteCustomer.IsEnabled = true;
                btnSaveCustomer.IsEnabled = false;
                btnCancelCustomer.IsEnabled = false;
                btnPrevCustomer.IsEnabled = true;
                btnNextCustomer.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);

                btnNewCustomer.IsEnabled = true;
                btnEditCustomer.IsEnabled = true;
                btnDeleteCustomer.IsEnabled = true;
                btnSaveCustomer.IsEnabled = false;
                btnCancelCustomer.IsEnabled = false;
                btnPrevCustomer.IsEnabled = true;
                btnNextCustomer.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                btnNewCustomer.IsEnabled = true;
                btnEditCustomer.IsEnabled = true;
                btnDeleteCustomer.IsEnabled = true;
                btnSaveCustomer.IsEnabled = false;
                btnCancelCustomer.IsEnabled = false;
                btnPrevCustomer.IsEnabled = true;
                btnNextCustomer.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
        }

        private void btnSaveInventory_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem entitatea Customer
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewInventory.IsEnabled = true;
                btnEditInventory.IsEnabled = true;
                btnDeleteInventory.IsEnabled = true;
                btnSaveInventory.IsEnabled = false;
                btnCancelInventory.IsEnabled = false;
                btnPrevInventory.IsEnabled = true;
                btnNextInventory.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = makeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);

                btnNewInventory.IsEnabled = true;
                btnEditInventory.IsEnabled = true;
                btnDeleteInventory.IsEnabled = true;
                btnSaveInventory.IsEnabled = false;
                btnCancelInventory.IsEnabled = false;
                btnPrevInventory.IsEnabled = true;
                btnNextInventory.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();

                btnNewInventory.IsEnabled = true;
                btnEditInventory.IsEnabled = true;
                btnDeleteInventory.IsEnabled = true;
                btnSaveInventory.IsEnabled = false;
                btnCancelInventory.IsEnabled = false;
                btnPrevInventory.IsEnabled = true;
                btnNextInventory.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem entitatea Customer
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewOrder.IsEnabled = true;
                btnEditOrder.IsEnabled = true;
                btnDeleteOrder.IsEnabled = true;
                btnSaveOrder.IsEnabled = false;
                btnCancelOrder.IsEnabled = false;
                btnPrevOrder.IsEnabled = true;
                btnNextOrder.IsEnabled = true;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                btnNewOrder.IsEnabled = true;
                btnEditOrder.IsEnabled = true;
                btnDeleteOrder.IsEnabled = true;
                btnSaveOrder.IsEnabled = false;
                btnCancelOrder.IsEnabled = false;
                btnPrevOrder.IsEnabled = true;
                btnNextOrder.IsEnabled = true;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewOrder.IsEnabled = true;
                btnEditOrder.IsEnabled = true;
                btnDeleteOrder.IsEnabled = true;
                btnSaveOrder.IsEnabled = false;
                btnCancelOrder.IsEnabled = false;
                btnPrevOrder.IsEnabled = true;
                btnNextOrder.IsEnabled = true;
            }
            SetValidationBinding();
        }

        private void btnNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewCustomer.IsEnabled = false;
            btnEditCustomer.IsEnabled = false;
            btnDeleteCustomer.IsEnabled = false;
            btnSaveCustomer.IsEnabled = true;
            btnCancelCustomer.IsEnabled = true;
            btnPrevCustomer.IsEnabled = false;
            btnNextCustomer.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnNewInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnSaveOrder.IsEnabled = true;
            btnCancelOrder.IsEnabled = true;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNewCustomer.IsEnabled = false;
            btnEditCustomer.IsEnabled = false;
            btnDeleteCustomer.IsEnabled = false;
            btnSaveCustomer.IsEnabled = true;
            btnCancelCustomer.IsEnabled = true;
            btnPrevCustomer.IsEnabled = false;
            btnNextCustomer.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnEditInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNewOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnSaveOrder.IsEnabled = true;
            btnCancelOrder.IsEnabled = true;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewCustomer.IsEnabled = false;
            btnEditCustomer.IsEnabled = false;
            btnDeleteCustomer.IsEnabled = false;
            btnSaveCustomer.IsEnabled = true;
            btnCancelCustomer.IsEnabled = true;
            btnPrevCustomer.IsEnabled = false;
            btnNextCustomer.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
        }

        private void btnDeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnSaveOrder.IsEnabled = true;
            btnCancelOrder.IsEnabled = true;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnCancelCustomer_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewCustomer.IsEnabled = true;
            btnEditCustomer.IsEnabled = true;
            btnDeleteCustomer.IsEnabled = true;
            btnSaveCustomer.IsEnabled = false;
            btnCancelCustomer.IsEnabled = false;
            btnPrevCustomer.IsEnabled = true;
            btnNextCustomer.IsEnabled = true;
            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
        }

        private void btnCancelInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewInventory.IsEnabled = true;
            btnEditInventory.IsEnabled = true;
            btnDeleteInventory.IsEnabled = true;
            btnSaveInventory.IsEnabled = false;
            btnCancelInventory.IsEnabled = false;
            btnPrevInventory.IsEnabled = true;
            btnNextInventory.IsEnabled = true;
            inventoryDataGrid.IsEnabled = true;
            makeTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewOrder.IsEnabled = true;
            btnEditOrder.IsEnabled = true;
            btnDeleteOrder.IsEnabled = true;
            btnSaveOrder.IsEnabled = false;
            btnCancelOrder.IsEnabled = false;
            btnPrevOrder.IsEnabled = true;
            btnNextOrder.IsEnabled = true;
        }

        private void btnPrevCustomer_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnPrevInventory_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnPrevOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextCustomer_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNextInventory_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNextOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnNextOrder_Clikc(object sender, RoutedEventArgs e)
        {

        }
    }
}
 