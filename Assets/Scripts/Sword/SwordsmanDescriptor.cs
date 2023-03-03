using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanDescriptor : MonoBehaviour
{
    public Transform InputClampingBoundary => inputClampingBoundary;

    [SerializeField] private Transform inputClampingBoundary;
}
