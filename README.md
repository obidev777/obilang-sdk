# obilang-sdk
ObiLangSDK - Es una sdk para el codigo de programacion (.obi) creado por obisoftdev

# Downloads
https://raw.githubusercontent.com/obidev777/obilang-sdk/main/obilang-sdk.rar

# Code Use - (windows environment variables path obilang-sdk)

# Basic Code
```
# libs imports
lib 'System'

# Uses
use System->Process

# Todo Code
Process::Start 'cmd'
```

# Window Gui
Create win.xml
```
<Form Text="NewForm" Width="600" Height="400">

<Button Name="Btn1" Text="Btn1"/>

</Form>
```
Create main.obi
```
#libs
lib 'gui.forms'
lib 'System.Windows.Forms'
lib 'System.Drawing'
#uses
use forms->XmlForm

#todo code
var gui : XmlForm::Load 'gui/win.xml'
var win : gui::Window

var btn1 : gui::get 'Btn1'

func b1_click sender,event
out btn1::Text : 'SET TEXT'
endfunc

out btn1::Click : (delegate)$b1_click

gui::Run
```
