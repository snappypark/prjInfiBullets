using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<KeyType, PriorityType> where PriorityType : System.IComparable
{
    struct Element<SubClassKeyType, SubClassPriorityType> where SubClassPriorityType : System.IComparable
    {
        public SubClassKeyType key;
        public SubClassPriorityType priority;

        public Element(SubClassKeyType key_, SubClassPriorityType priority_)
        {
            this.key = key_;
            this.priority = priority_;
        }
    }

    List<Element<KeyType, PriorityType>> _queue = new List<Element<KeyType, PriorityType>>();

    public void Clear()
    {
        _queue.Clear();
    }

    public bool isEmpty()
    {
        return _queue.Count == 0;
    }

    public void push(KeyType arg_key, PriorityType arg_priority)
    {
        Element<KeyType, PriorityType> new_elem = new Element<KeyType, PriorityType>(arg_key, arg_priority);

        int index = 0;
        foreach (var element in _queue)
        {
            if (new_elem.priority.CompareTo(element.priority) < 0)
                break;
            ++index;
        }
        
        _queue.Insert(index, new_elem);
    }

    public KeyType pop()
    {
        if (isEmpty())
            throw new UnityException("Attempted to pop off an empty queue");
        Element<KeyType, PriorityType> top = _queue[0];
        _queue.RemoveAt(0);
        return top.key;
    }
}