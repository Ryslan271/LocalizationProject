using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationProject
{
    internal class Class1 : IEditableCollectionView
    {
        public bool CanAddNew => throw new NotImplementedException();

        public bool CanCancelEdit => throw new NotImplementedException();

        public bool CanRemove => throw new NotImplementedException();

        public object CurrentAddItem => throw new NotImplementedException();

        public object CurrentEditItem => throw new NotImplementedException();

        public bool IsAddingNew => throw new NotImplementedException();

        public bool IsEditingItem => throw new NotImplementedException();

        public NewItemPlaceholderPosition NewItemPlaceholderPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelNew()
        {
            throw new NotImplementedException();
        }

        public void CommitEdit()
        {
            throw new NotImplementedException();
        }

        public void CommitNew()
        {
            throw new NotImplementedException();
        }

        public void EditItem(object item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
