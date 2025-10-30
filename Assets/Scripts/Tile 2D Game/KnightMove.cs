using System.Collections.Generic;
using UnityEngine;

public class KnightMove : MonoBehaviour
{
    public float movingInterval = 0.5f;

    private List<Tile> movingRoute;
    private bool isMove = false;
    public bool IsMove => isMove;
    private int pos = 0;
    private Stage stage;

    public void SetRoute(List<Tile> route, Stage stage)
    {
        movingRoute = route;
        this.stage = stage;
        isMove = true;
    }

    private float movingTime;

    // Update is called once per frame
    void Update()
    {
        
    }
}
