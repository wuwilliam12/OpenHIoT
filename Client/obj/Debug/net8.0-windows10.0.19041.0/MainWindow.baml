   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:OpenHIoT.ClientOpenHIoT.Client  5   	        OpenHIoT.Client.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:OpenHIoT.Client     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��139* �6   �� 5      $
��295* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   ��  �� Orientation$ Vertical=�6   $��0��6,   
�������$��2��6T   .+�5      �� $��40��6   $
��Left=�6!   
�������$��0q�6T   .��5      �� $ 
Horizontal=�6   .+�5      �� $'��!/Client;component/Images/user.jpg��6   �� 5      $��10q�6   $��14(�6)   $��Center=�68   $��64��6Z   .��6e   User6k   5      5      �� 5      $��40��6   $
��Left=�6!   
�������$��0q�6T   .��5      �� $ 
Horizontal=�6   .+�5      �� $&�� /Client;component/Images/UNS.jpg��6   $��40��6F   �� 5      $��10q�6   $��Center=�6+   $��60��6M   .��6Y   UNS6^   5      5       �� 5!      -   5        $��40��5!      $
��Left=�6!   
�������$��0q�6T   .��5"      �� $ 
Horizontal=�6   .+�5#      �� $(��"/Client;component/Images/chart.png��6   �� 5$      $��Center=�6   $��65��6>   .��6I   	Display6R   5%      5&      �� 5'      -   5        $��40��5'      $
��Left=�6!   $��14(�6<   
�������$��0q�6b   .��5(      �� $ 
Horizontal=�6   $��40��66   .+�5)      �� $*��$/Client;component/Images/devices.png��6   �� 5*      $��10q�6   $��14(�6)   $��Center=�68   $��60��6Z   .��6f   Device6n   5+      5,      5/       � 50   
   -   5          �Name$ 	mainFrame��50   !   $��1��6   $��2��64   �� 51   
   $��2��6   $��5��6(   $��Center=�63   $��2��6P       OpenHIoT.Client.SysLogCntl  52   
   -   5          Name$ sysLog��52      $��3��6+   $��2��6;   54      55      