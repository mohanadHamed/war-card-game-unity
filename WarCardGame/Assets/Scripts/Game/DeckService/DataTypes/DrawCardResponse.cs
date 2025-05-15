using System;
using System.Collections.Generic;

[Serializable]
public class DrawCardResponse
{
    public bool success;
    public string deck_id;
    public List<CardData> cards;
    public int remaining;
}
