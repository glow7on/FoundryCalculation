   M S B A M L   `   `   `  ����M  IFoundryCalculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:FoundryCalculationFoundryCalculation  5   	   "    FoundryCalculation.StartPage   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:FoundryCalculation     5   	     � Icon$  /Resources/Icon.png��5   	    � Title$! Стартовое окно��5	   	   $	��455��5
   	   $	��650��6   ���	#FF1E1E1E.��5      � ���	#FF202020$��0,0,0,-6q�6"   .+�5   
   �� -   5         ��Name$ CreateFileButton��5      $��Center=�6+   $��236,120,0,0q�6M   $��24(�6b   
����������	#FF424040$	��232��6�   $	��Top=�6�   $
��Left=�6�   $	��174��6�   .��5      �� $��/Resources/NewFile.png��6   $	��140��64   $	��100��6A   5      �� 5   
   -   5        $ OpenDataButton��5      $��307,120,0,0q�6)   ���	#FF424040$��72��6U   $	��Top=�6a   $
��Left=�6y   $	��283��6�   $y�Hidden=�6�   .��5      �� $��Resources/LoadFile.png��6   $��54��64   $
��Left=�6@   $��60��6[   $��
-120,0,0,0q�6f   $
��Fill=�6z   5      �� 5   
   -   5        $ AboutButton��5      $��О программе��6&   $��307,206,0,0q�6<   ���	#FF424040$��18(�6h   
�������$��Yu Gothic UI Light)�6�   $��60��6�   $	��Top=�6�   $
��Left=�6�   $	��194��6�   $y�Hidden=�6�   �� 5   
   $!��Создать шаблон��6   $��	0,311,0,0q�6)   $��18(�6<   $��Yu Gothic UI Light)�6J   
�������$��37��6}   $	��Top=�6�   $��Center=�6�   $	��138��6�   �� 5   
   $*��$База данных сплавов��6   $��402,140,0,0q�6.   $��Yu Gothic UI Light)�6C   $��18(�6c   
�������$��36��6�   $	��Top=�6�   $
��Left=�6�   $	��188��6�   $y�Hidden=�6�   � 5   
   ���	#FF262626$��62��6&   $	��Top=�62   .+�5      �� $��Foundry Calculation��6   
�������$��20,10,366,0q�6E   $	��Top=�6Z   $��22(�6r   �� 5      -   5         ��Name$ 
AppVersion��5      $�� ��6(   
�������$��566,21,10,0q�6F   $	��Top=�6[   $��16(�6s   5      �� 5   
   $��Начало работы��6   $
��Left=�6(   $��
231,79,0,0q�6C   $	��Top=�6W   
�������$��17(�6�   $	��179��6�   �� 5   
   $ ��Дополнительно��6   $
��Left=�6(   $��
307,79,0,0q�6C   $	��Top=�6W   
�������$��17(�6�   $	��161��6�   $y�Hidden=�6�   �� 5   
   $��	0,378,0,0q�6   $��28��6'   $	��Top=�63   $��Center=�6K   $	��178��6h   $��13(�6t   �� 5      -   5        $��FormCalculation.xaml?�5      .�6]   /-Продолжить без шаблона ->6x   5      5      5       