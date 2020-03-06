extends FileDialog

export var labelPath : NodePath;
var status : Label;
var loadedScene : PackedScene;

func _ready():
	status = get_node(labelPath);

func _on_MenuButton_button_up():
	popup();


func _on_FileDialog_file_selected(path):
	loadedScene = load(path);
	print(loadedScene);
	status.visible = true;
	status.text = "Loaded Scene";
	
