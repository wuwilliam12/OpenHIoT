   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:OpenHIoT.Client.Pages.Channel.DataList&OpenHIoT.Client.Pages.Channel.DataList  5      <    6OpenHIoT.Client.Pages.Channel.DataList.ListViewControl   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Flocal4clr-namespace:OpenHIoT.Client.Pages.Channel.DataList     5      .��5	      � 
�������$��5q�6"   $��Stretch=�6-     � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6   .+�5      �� -   5         ��Name$ 
titleLabel��5   2   $��Center=�6   6F   �� 5      -   5         ��Name$ 
settingBtn��5      $��25��6*   $��Right=�66   $��Bottom=�6R   
������ 
������ .��5      �� $*��$/Client;component/Images/setting.png��6   5      5      � 5   
   -   5         �Name$ mainGd��5      $��1��6   .+�5      �� -   5         ��Name$ itemList��5      $��Stretch=�6(   ��5      �� 5      .R�5      � $	��0,2q�6     5      �� 5      $
��Auto �6,   �� 5      $
��Auto �6,   .+�5       � $��0��6    � ColumnDefinitions 5!   "   �� 5"   "   $
��Auto �63   �� 5#   "   $
��Auto �63   �� 5$   "   $
��Auto �63   �� 5%   "   $
��Auto �63   �� 5&   "   $��* �63   .+�5(   "   �� $��0��6,   ��6<   �� *Channel.Name+�� 5)   "   $��1��6,   $��0,0,5,0q�6Q   ��6<   �� *UOM+6Q   �� 5*   "   $��2��6)   
�������._�5+   &   �� $��Center=�6�   $��0,0,5,0q�6�   ��61   �� *Val+��6F   �� *Brush+y�6c   �� *	ShowVal+6�   5,   #   �� 5-   "   $��3��6)   
�������._�5.   &   :   4OpenHIoT.Client.Pages.Channel.DataList.OptionsRadios  $��Center=�6m   $��0,0,5,0q�6�      Options 6;   �� *	Options+
   Val 6X   �� *Val+6�   5/   #   �� 50   "   $��4��6)   $��1q�69   
�������$��0,0,5,0q�6`   ._�51   &   6   0OpenHIoT.Client.Pages.Channel.DataList.CurveCntl     Sample 66   �� *Sample+	   Setting	 6P   �� *CurveSetting+52   "   56      58      59      5;      �� 5<      -   5        $ 	addNewBtn��5<      $��30��6)   $��Right=�65   $��Bottom=�6Q   $��10q�5=   1   
������ 
������ .��5>      �� $*��$/Client;component/Images/add_new.png��6   5?      �� 5@      -   5        $ delBtn��5@      $��30��6&   $
��Left=�62   $��Bottom=�6M   $��10q�5A   .   
������ 
������ .��5B      �� $)��#/Client;component/Images/delete.png��6   5C      5D      � 5E   
   -   5        $ 	settingGd��5E      $��1��6   6   5F      5G      