   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	    IRC   IRC.AddRoomWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	     � Title$  AddRoomWindow��5   	   $	��215��6   $	��327��6,   .��5      � 
���   �$��0,0,2,-2q�6   .+�5   
   �� -   5         ��Name$ nazwa��5      $��Wpisz nazwę pokoju:��6   $
��Left=�6=   $��	24,35,0,0q�6X   $	��Top=�6k   
�������$��Arial)�6�   $
��Bold"�6�   �� 5   
   -   5        $	 max��5      $)��#Określi maksymalną liczbę osób:��6   $
��Left=�6G   $��	24,73,0,0q�6b   $	��Top=�6u   
�������$��Arial)�6�   $
��Bold"�6�   $	��201��6�   �� 5	   
   -   5         ��Name$ RoomName��5	      $
��Left=�6$   $��23��6?   $��
160,35,0,0q�6K    �� TextWrapping$
 Wrap=�6_   $	��Top=�6s   $	��147��6�   $
��Bold"�6�   $��Arial)�6�   �� 5
   
   -   5         ��Name$ 
CreateRoom��5
      $��Stwórz!��6%   $
��Left=�67   $��128,139,0,0q�6R   $	��Top=�6g   $��75��6   ���	#FF1753A1$��Arial)�6�   $��13(�6�   $
��Bold"�6�   6�   %   Xceed.Wpf.Toolkit.IntegerUpDown  5   
   -   5          Name$ OccupancyInteger��5       $��0��6:   $��230,65,10,88q�6G   �   �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin . ,   &Xceed.Wpf.Toolkit.Primitives.InputBase   AllowTextInput .     DefaultValue$ 10X�6�   %	   DisplayDefaultValueOnEmptyText	 . 
   Minimum$
 2X�6�      Maximum$	 200X�6�      MouseWheelActiveTrigger$ 	MouseOver=�6�   5      5      5      