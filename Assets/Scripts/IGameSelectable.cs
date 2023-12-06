using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSelectable
{
    void Select();
    void Deselect();
    bool IsSelected();
}
