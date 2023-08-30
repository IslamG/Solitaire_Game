//using UnityEngine.Events;
////Custom event for triggering when a card is moved
////for calling drop spot item action
//public class OnCardMoved<T> : UnityEvent<T> { }
////public class OnCardMoved<T> (T src) : UnityEvnet<T>{ }
//public class OnCardMoved<T, T1> : UnityEvent<T, T1>
//{
//}
//public class OnCardMovedFromGroup : OnCardMoved<CardGroup>
//{
//}
//public class OnCardMovedToGroup : OnCardMoved<Card, CardGroup>
//{
//}

//public class OnCardMoved<CardSpace> : UnityEvent<CardSpace> { }
//public class OnCardsMoved<List> : UnityEvent<List> { }
//public class OnCardMovedToGroup<CardSpace> : UnityEvent<CardSpace> { }
//public class OnCardsMovedToGroup<List> : UnityEvent<List, CardGroup> { }
using UnityEngine.Events;

public class OnCardMovedToFoundation<CardSpace, Foundation> : UnityEvent<CardSpace, CardGroup> { }//Tab +-> Found, group == dst
public class OnCardMovedToTablue<CardSpace, Tablue> : UnityEvent<CardSpace, CardGroup> { } //Tab +-> Tab, group == dst
public class OnCardListMovedToTablue<List, Tablue> : UnityEvent<List, CardGroup> { } //Tab +[] -> Tab, group == dst
public class OnCardLeftTablue<CardSpace, Tablue> : UnityEvent<CardSpace, CardGroup> { }//Tab - -> Tab, group == src
public class OnCardListLeftTablue<List, Tablue> : UnityEvent<List, CardGroup> { }//Tab -[] -> Tab, group = src

public class OnCardLeftDiscard<CardSpace, DiscardPile> : UnityEvent<CardSpace, CardGroup> { } //waste -> Found || Tab, group == dst
public class OnCardExposed: UnityEvent { }
public class OnDiscardReset : UnityEvent { }
public class OnFoundationMaxed: UnityEvent { }