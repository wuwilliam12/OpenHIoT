   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:OpenHIoT.Client.Pages.DeviceOpenHIoT.Client.Pages.Device  5      9!clr-namespace:TreeGridView.CommonTreeGridView.Common  5      1    +OpenHIoT.Client.Pages.Device.DeviceMainPage   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:OpenHIoT.Client.Pages.Device     5      7	tree_grid!clr-namespace:TreeGridView.Common     5        N� Title$  
DevicePage��5
      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �  � IsSharedSizeScope$
 True��6   $��0��6.    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      .+�5      Q� $��0��6   $��1q�6(   ��5      � 5       5      �� 5      $	��100 �6+   69   �� 5      $
��Auto �6+   $��50��68   6H   �� 5      6.   .+�5      �� $
��Name��6    $
��Left=�6-   $��Center=�6H   $��0��6c   �� 5      $
��NsId��6    $��	10,0,10,0q�6-   $
��Left=�6@   $��Center=�6[   $��1��6w   �� 5       $��Physical Id��6    $
��Left=�64   $��Center=�6P   $��2��6l   5!      5#      R� 5$   
   -   5         R�Name$ treeviewList��5$      $��1��6*   $	��340��68   ��5%      �� 5&       �� ItemsSource 6,   �� *
Children+.R�5'   "   �  5(   &   �� 5)   *   $	��100 �6;   6I   �� 5*   *   $
��Auto �6;   $��50��6H   6X   �� 5+   *   6=   .+�5-   &   �� $��10,0,0,0q�6o   $
��Left=�6�   $��Center=�6�   $��0��6�   ��60   �� *Device.Name+��6M   �� *	NameColor+6�   �� 5.   &   $��	10,0,10,0q�6o   $
��Left=�6�   $��Center=�6�   $��1��6�   ��60   �� *Device.NsId+��6M   �� *	NameColor+6�   �� 5/   &   $
��Left=�6i   $��Center=�6�   $��2��6�   ��60   �� *PhyId+��6G   �� *	NameColor+6�   50   #   51      54      55   	   �� 56   
   -   5        $��0��56      $��30��6:   $��Right=�6F   
������ 
������  � ToolTip$ Refresh device list��58      $��5q�6,   $��Bottom=�68   .��59      �� $*��$/Client;component/Images/refresh.png��6   5:      �� 5;   
   -   5         ��Name$ delBtn��5;      $��30��6"   $
��Left=�6.   $��Bottom=�6I   $��5q�5<   .   
������ 
������ .��5=      �� $)��#/Client;component/Images/delete.png��6   5>      � 5?   
   -   5         �Name$ 
detailCntl��5?      $��1��6   6   5@      5A      