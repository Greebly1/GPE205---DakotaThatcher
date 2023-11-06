using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Room : MonoBehaviour {

	[SerializeField] private GameObject NorthDoor;
    [SerializeField] private GameObject EastDoor;
    [SerializeField] private GameObject SouthDoor;
    [SerializeField] private GameObject WestDoor;

    public DoorState[] states;

    //indexes of the 4 doors
    //0 = north, 1 = east, 2 = south, 3 = west
    private GameObject[] doors = new GameObject[4];

        public void Awake()
    {
        doors[0] = NorthDoor;
        doors[1] = EastDoor;
        doors[2] = SouthDoor;
        doors[3] = WestDoor;
    }
    public void setDoors(DoorState[] doorStates)
	{
        this.states = doorStates;
		for (int door = 0; door < 4; door++)
		{
            if (doorStates[door] == DoorState.open)
            {
                Destroy(doors[door]);
            }
		}
		
	}
}
