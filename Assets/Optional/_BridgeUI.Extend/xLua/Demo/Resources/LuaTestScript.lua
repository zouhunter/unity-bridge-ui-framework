local btn_color = 001;
local btn_color1 = 002;
local on_cull_statechanged = 003;
local on_cull_statechanged1 = 004;
local on_button_clicked = 005;
local on_button_clicked1 = 006;
local on_slider_switched = 007;
local on_slider_switched1 = 008;
local on_inputfield_edited = 009;
local on_inputfield_edited1 = 010;
local on_dropdown_switched = 011;
local on_toggle_switched = 012;
local image = 013;
local image_color = 014;
local text = 015;
local on_scrollview_changed = 016;
local Debug = CS.UnityEngine.Debug

function on_initialize(panel)
	print("lua oninit...")
	--print(self.keyword_btn_color)
	--slider_trans = self.transform:Find("Slider")
	--slider = slider_trans:GetComponent(typeof(CS.UnityEngine.UI.Slider))
	--slider.onValueChanged:AddListener(printprogress)
	self:SetValue(text,"我是从lua中生成的文字")
	self:SetValue(image_color,CS.UnityEngine.Color(1,0,0,1))
end

function on_update()
	print("lua update...")
	self:SetValue(btn_color,CS.UnityEngine.Color(CS.UnityEngine.Time.time % 1,0.5,0.1,1))
end

function on_recover()
    print("lua destroy")
end

function printprogress(f)
	print(f)
end

function on_button_clicked(panel)
	print("lua on_button_clicked...")
	buttonClicked = true
	Debug.Log("(debug:)lua on_button_clicked...")
end

function on_toggle_switched(panel,isOn)
	print("lua on_toggle_switched...",isOn)
end

--下拉框
function on_dropdown_switched(panel,value)
	print("lua on_dropdown_switched...",value)
end

--输入框
function on_inputfield_edited(panel,text)
	print("lua on_inputfield_edited...",text)
end

--滑动框
function on_scrollview_changed(panel)
	print("lua on_scrollview_changed...")
end

function on_receive(data)
	print(data)
end