﻿var $url = '/admin/shared/editorLayerAudio';

var data = utils.initData({
  attributeName: utils.getQueryString('attributeName'),
  form: {
    siteId: utils.getQueryInt('siteId'),
    type: 'upload',
    audioUrl: '',
    isAutoPlay: false
  },
  uploadUrl: null
});

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (!this.form.audioUrl) {
      this.$message.error('请上传需要插入的音频文件！');
      return false;
    }

    var isAutoPlay = ' isAutoPlay="' + this.form.isAutoPlay + '"';

    parent.$vue.insertEditor($this.attributeName, '<img ' + isAutoPlay + ' playUrl="' + this.form.audioUrl + '" style="width: 400px; height: 40px;" class="siteserver-stl-audio" src="../assets/ueditor/audio-clip.png" /><br/>');
    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadAudioBefore(file) {
    var re = /(\.mp3)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是音频格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadAudioSuccess: function(res) {
    this.form.audioUrl = res.url;
    this.form.type = 'url';
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId;
    utils.loading(this, false);
  }
});