   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:RealTimeGraphX.WPFRealTimeGraphX.WPF  5      g8clr-namespace:OpenHIoT.Client.Pages.Channel.Live.ChartRt*OpenHIoT.Client.Pages.Channel.Live.ChartRt  5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089@    :OpenHIoT.Client.Pages.Channel.Live.ChartRt.WpfGraphControl  �� 
TargetType �� Property �� Value �� EndPoint �� 
StartPoint � Color � Offset �� 
TargetType �� CornerRadius	 �� Padding
 � RowDefinitions � ColumnDefinitions*   $RealTimeGraphX.WPF.WpfGraphGridLines    
Controller.   (RealTimeGraphX.WPF.WpfGraphComponentBase   
Controller(   "RealTimeGraphX.WPF.WpfGraphSurface  Name   
Controller    DisplayToolTip   DisplayToolTip,   &RealTimeGraphX.WPF.WpfGraphAxisControl   Orientation    StringFormatX   StringFormat    StringFormatY J� Opacity �� RelativeSource �� AncestorType �� Path �� StringFormat �Name   Image ��Name ��Name
 �� Top  �� Left! ��Name" �� Triggers# �� Binding$ �� Mode% �� Value& ��Name'   ToolTipTemplate( ��Name)   Items*   ItemTemplate*   $RealTimeGraphX.WPF.WpfGraphAxisPanel+  Name,   Orientation- P� Property. P� Value/ �� DataType-   'RealTimeGraphX.WPF.WpfGraphAxisTickData0 J� RenderTransformOrigin1 �� Angle2   HorizontalItems3 A� Columns4   VerticalItems5 A� Rows�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      3realTimeGraphX clr-namespace:RealTimeGraphX.WPF  5      DrtChart8clr-namespace:OpenHIoT.Client.Pages.Channel.Live.ChartRt  5      %!  '         '  �    '  �	    '  J    �� 5      "    6   .�5	   
   ��  �@  StringFormatX$	 0.0��6*   68   �� 5
   
    �@  StringFormatY$ 
hh\:mm\:ss��6*   6?   �� 5   
    � ��$ 1q�6,   68   �� 5   
    � �� �	#FF2E2E2E6<   �� 5   
    � ��$ 20 20 30 20q�6$   6:   �� 5   
    � ��$ 11(�6%   62   �� 5   
    � ��
 ����6<   �� 5   
    � �� 5      �� 5      $ 0.5,1&�6&   $ 0.5,0&�67   .��5      � $ Black��6#   � 5      $ 	#FF333333��6#   $ 1Q�65   5      5      �� 5   
    � �� 5      �� 5      "   6"   .R�5      �� #��z��6   #��z��5      #��z��5      $ 5��5      #	 z��5      ._�5       � 
 5!      �� 5"   "   $��1* �60   �� 5#   "   $��35 �60    5%      �� 5&   "   $��70 �63   �� 5'   "   $��1* �63   .+�5*      � $��1��6#   .+�5+   "     # z 6C   ���	#FF2E2E2E6m     5,   "   $ surface��6A   # z 6R   # z 6|   $��1 0 0 1q�6�   #��z��6�   5-        5/      $ Vertical=�6A   # z 6X   # z 6�     50      $ 
Horizontal=�6A   # z 6Z   $��1��6�   $��1��6�   # z 6�   51      52      53      55      56      �� 58      "   6   .�59   
   ��  �@ ToolTipTemplate 5:      �� 5;      .R�5<      �� $	 0.8Q�5=      $��1q�5>      $
	 10 5q�5?      $ 5��5@      ���#151515��5A      ��  �� " realTimeGraphX:WpfGraphSurface$ BorderBrush�._�5D      �� ��6$   �� $	 0.0��6H   5E      5F      5H      �� 5I   
    � �� 5J      �� 5K      "  6"   .R�5L      �� #��z��6   #��z��5M      #��z��5N      ._�5O      � $ 	PART_Grid��6   #��z��62   .+�5P      �� #��z 6$   $
��None=�6E   6V   �� 5R      $ PART_SelectionCanvas��6%   
������ .+�5S   "   �� $ PART_SelectionRectangle��6,   ���	#7BFFFFFF$ 0��6^   $  0��6m   $��0��6}   $��0��6�   $y�Hidden=�6�   6�   �� 5U   "   $! PART_BorderToolTip��6)   $ 0Q�6E   ��5V   &   �� 5W   *   "  ��60   .�5X   .   ��  � y�$ 	Collapsed=�6K   6_   " 5Y   .   �� 5Z   2   # 6>   ��  �� $$ TemplatedParent=�$ DisplayToolTip�% True6�   .a�5[   6   ��  � y�$ Visible=�6S   6e   5\   3   5^   +   ._�5a   &   �� $& PART_ToolTipContent��65   #��z' 6R   6�   5b   #   5c      5d      5e      5f      5h      5i      �� 5k      "   6   .�5l   
   ��  � �� 5m      �� 5n      "  6"   .R�5o      �� #��z��6   #��z��5p      #��z��5q      ._�5s      �� $( PART_ItemsControl��6'   #��z) 6B   #��z* 6h   ��5t      �� 5u   "   .R�5v   &     $+ PART_AxisPanel��6G   , 6_   ��  �� & "realTimeGraphX:WpfGraphAxisControl$ Orientation�6�   5w   #   5y      5z      5{      5}      " 5~   
   P� 5      - �@ Orientation$. Vertical=�6-   . �5�      ��  �@ ItemTemplate 5�      �� 5�      "/  6'   .R�5�      ]� $	��Top=�6(   .+�5�   "   �� $��Right=�6,   $��Center=�6C   $��1��6�   ��6^   ��  �� & "realTimeGraphX:WpfGraphAxisControl$ 
Foreground���5�   &   �� 5�   *   "  ��60   .�5�   .   ��  � ��$ 4��6F   6R   " 5�   .   �� 5�   2   # 6>   �� *IsEven+% True6Y   .a�5�   6   ��  � ��$ 6��6N   6Z   5�   3   5�   +   5�   #   �� 5�   "   ��6,   �� *DisplayText+6K   5�      5�      5�      5�      P� 5�      - �@ Orientation$. 
Horizontal=�6-   . �5�      ��  �@ ItemTemplate 5�      �� 5�      "/  6'   .R�5�      ]� $
��Left=�6(   .+�5�   "   �� $	��Top=�6,   $��Center=�6A   $��1��6�   ��6^   ��  �� & "realTimeGraphX:WpfGraphAxisControl$ 
Foreground���5�   &   �� 5�   *   "  ��60   .�5�   .   ��  � ��$ 4��6G   6S   " 5�   .   �� 5�   2   # 6>   �� *IsEven+% True6Y   .a�5�   6   ��  � ��$ 6��6O   6[   5�   3   5�   +   5�   #   �� 5�   "   $��Bottom=�6,   $0 0.5,0.5&�6d   ��6G   �� *DisplayText+z�5�   &   �� 5�   *   $	1 -30Q�6:   5�   #   5�      5�      5�      5�      5�      �� 5�      "   6   .�5�   
   ��  � �� 5�      �� 5�      "  6"   .R�5�      �� #��z��6   #��z��5�      #��z��5�      ._�5�      � .+�5�      �� #��z2 6+   ��5�   "   �� 5�   &   .R�5�   *   A� 3 66   ��  �� $  realTimeGraphX:WpfGraphGridLines$ Columns�6�   5�   '   ��5�   "   �� 5�   &   .R�5�   *   �� $��1��6�   $
��Left=�6�   $��Stretch=�6�   ��64   ��  �� $  realTimeGraphX:WpfGraphGridLines$ 
Foreground�6�   5�   '   5�      �� 5�      #��z4 6+   ��5�   "   �� 5�   &   .R�5�   *   A� 5 66   ��  �� $  realTimeGraphX:WpfGraphGridLines$
 Rows�6�   5�   '   ��5�   "   �� 5�   &   .R�5�   *   �� $��1��6�   $��Stretch=�6�   $	��Top=�6�   ��64   ��  �� $  realTimeGraphX:WpfGraphGridLines$ 
Foreground�6�   5�   '   5�      �� 5�      $��1��6�   $��Right=�6�   $��Stretch=�6�   ��6(   ��  �� $  realTimeGraphX:WpfGraphGridLines$ 
Foreground�6�   �� 5�      $��1��6�   $��Stretch=�6�   $��Bottom=�6�   ��6(   ��  �� $  realTimeGraphX:WpfGraphGridLines$ 
Foreground�6�   5�      5�      5�      5�      5�      5�      