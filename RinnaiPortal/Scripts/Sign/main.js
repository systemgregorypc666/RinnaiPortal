/**=[對話匡處理]==========================================================*/
var Dialog = {
    observes: {},
 
    on: function (event, observe) {
        if (!this.observes[event]) {
            this.observes[event] = [];
        }
        this.observes[event].push(observe);
        return this;
    },
 
    exec: function (event, data) {
        if (!this.observes[event]) { return this; }
        for (var i = this.observes[event].length - 1; i >= 0; i--) {
            this.observes[event][i](data);
        };
 
        return this;
    },
 
    trigger: function (event, data) {
        if (window.parent != window && window.parent['Dialog']) {
            window.parent.Dialog.exec(event, data);
        }
        return this;
    },
 
    resize: function (width, height) {
        if(window.opener){
            window.resizeTo(width,height);
             
        } else if (window.parent != window && window.parent['Dialog']) {
            window.parent.Dialog.resize(width, height);
             
        } else {
            var winWidth = $(window).width();           
            var winHeight = $(window).height();     
             
            if(width > winWidth && winWidth < 760){
                width = 'auto';
            }else{
                width = Math.min(winWidth - 100, width);
            }
            height = Math.min(winHeight - 100, height);
             
            $('#DialogWidget .modal-dialog').css('width', width);
            $('#DialogWidget .modal-content').css('height', height);
             
        }
        return this;
    },
 
    open: function (url, type) {
 
        var $modal = $(
            '<div id="DialogWidget" class="modal fade">'+
                '<div class="modal-dialog">'+
                    '<span class="modal-dismiss" data-dismiss="modal">'+
                        '<span class="glyphicon glyphicon-remove" aria-hidden="true"></span>' +
                        '<span class="glyphicon glyphicon-remove-sign" aria-hidden="true"></span>' +
                    '</span>'+
                    '<div class="modal-content">'+
                        '<i class="glyphicon glyphicon-refresh"></i>' +
                        '<iframe></iframe>'+
                    '</div>'+
                '</div>'+
            '</div>'
        );
        $modal.attr({
            'aria-labelledby':'GfcDialog',
            'aria-hidden':'true',
            'tabindex':'-1',
            'role':'dialog'
        });
         
         
        $modal.find('iframe').attr({
            'allowTransparency':'true',
            'frameborder':'0',
            'scrolling':'no',
            'tabindex':'-1',
            'src': url            
        });
 
         
        $modal.modal({
            backdrop: 'static', keyboard: true
        }).on('hidden.bs.modal', function () {
            $(this).remove();
        });
 
        return $modal;
    },
 
    close: function () {
        if (window.opener) {
            window.close();
             
        } else if (window.parent != window && window.parent['Dialog']) {
            window.parent.Dialog.close();
             
        } else {
            $('#DialogWidget').modal('hide');
        }
 
        return this;
    }
};
 
/*訊息*/
Dialog.on('alert.status-msg', function (msg) {
    StatusMsg.alert(msg);
}).on('error.status-msg', function (msg) {
    StatusMsg.error(msg);
});
 
 
jQuery(function ($) {
 
    /* Dialog */
    $(document).on('click', "a[target=dialog]", function (event) {
        event.preventDefault();
        Dialog.open(this.href);
    });
 
    $(document).on('click', "a[target=window]", function (event) {
        event.preventDefault();
        var subwin = window.open(this.href, 'subwin_'+getGfcSeq(), 'top=0,left=0,height=150,width=150,scrollbars=no,resizable=yes');
        subwin.focus(); 
    });
 
 
    $('#DialogLayout').delegate('[data-dismiss="modal"]', 'click.dismiss.modal', Dialog.close);
 
    if ((window.opener && window.opener['Dialog']) ||
        (window.parent != window && window.parent['Dialog'])
    ) {
        $('body').on('keyup.dismiss.modal', function (e) {
            e.which == 27 && Dialog.close();
        });
    }
});
 