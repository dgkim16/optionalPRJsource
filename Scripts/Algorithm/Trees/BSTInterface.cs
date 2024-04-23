using System.Collections.Generic;

public interface BSTInterface<T>
{
    public void insert(T data);
    public bool remove(T data);
    public List<T> dataInOrder();
    public List<T> dataInOrderFilter(bool isPlayable);
    public List<T> dataPreOrder();
    public List<T> dataPostOrder();
    public List<T> dataLevelOrder();
    

    public void setCompareIndex(int mode);
    
}
