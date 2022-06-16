/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
	//允許所有的html tag, 不讓套件自動亂改
	config.allowedContent = true;
	//工具欄是否可以被收縮
	config.toolbarCanCollapse = true;
	//工具欄默認是否展開
	config.toolbarStartupExpanded = true;
	//height
	config.height = 350;
	//定義fontsize
	config.fontSize_sizes = '12/12px;13/13px;16/16px;15/15px;18/18px;20/20px;22/22px;24/24px;36/36px;48/48px;';
	//fontname
	config.font_names = 'Arial;Arial Black;Comic Sans MS;Courier New;Tahoma;Times New Roman;Verdana;新細明體;細明體;標楷體;微軟正黑體';
    
    config.toolbarGroups = [
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'links', groups: ['links'] },
		'/',
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		{ name: 'insert', groups: ['insert'] },
		'/',
		{ name: 'styles', groups: ['styles'] },
		{ name: 'colors', groups: ['colors'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
    ];

    config.removeButtons = 'Save,Print,NewPage,Scayt,Form,Textarea,HiddenField,Checkbox,Select,Button,Radio,TextField,ImageButton,BidiLtr,BidiRtl,Language,Flash,Smiley,About';
};