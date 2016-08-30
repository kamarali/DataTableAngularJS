/*
Copyright (C) 2011 by Mal Curtis

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

if (typeof jQuery == 'undefined') throw("jQuery Required");

(function ($) {
  // Public General plug-in methods $.DirtyForms
  $.extend({
    DirtyForms: {
      debug: false,
      message: 'You\'ve made changes on this page which aren\'t saved. If you leave you will lose these changes.',
      title: 'Are you sure you want to do that?',
      dirtyClass: 'dirty',
      listeningClass: 'dirtylisten',
      ignoreClass: 'ignoredirty',
      helpers: [],
      dialog: {
        selector: '#pendingChangesDialog',
        // Fire starts the dialog
        fire: function (message, title) {
          $('#pendingChangesDialog').dialog({ closeOnEscape: false, modal: true, title: 'Unsaved changes' });
        },
        // Bind binds the continue and cancel functions to the correct links
        bind: function () {
          $('#pendingChangesDialog .cancel, #pendingChangesDialog .close').click(decidingCancel);
          $('#pendingChangesDialog .continue').click(decidingContinue);
          $(document).bind('decidingcancelled.dirtyform', function () {
            try {  
                $('#pendingChangesDialog').dialog('close');
            } catch (error) { }
          });
        },
        // Re-fire handles closing an existing dialog AND fires a new one
        refire: function (content) {
          var rebox = function () {
            $('#pendingChangesDialog').dialog({ closeOnEscape: false, modal: true, title: 'Unsaved changes' });
          }
        },
        // Stash returns the current contents of a dialog to be re-fired after the confirmation
        // Use to store the current dialog, when it's about to be replaced with the confirmation dialog. This function can return false if you don't wish to stash anything.
        stash: function () {
          return false;
        }
      },

      isDirty: function () {
        dirtylog('Core isDirty is starting ');
        var isDirty = false;
        $(':dirtylistening').each(function () {
          if ($(this).isDirty()) {
            isDirty = true;
            return true;
          }
        });

        $.each($.DirtyForms.helpers, function (key, obj) {
          if ("isDirty" in obj) {
            if (obj.isDirty()) {
              isDirty = true;
              return true;
            }
          }
        });

        dirtylog('Core isDirty is returning ' + isDirty);
        return isDirty;
      }
    }
  });

  // Create a custom selector $('form:dirty')
  $.extend($.expr[":"], {
    dirtylistening: function (a) {
      return $(a).hasClass($.DirtyForms.listeningClass);
    },
    dirty: function (a) {
      return $(a).hasClass($.DirtyForms.dirtyClass);
    }
  });

  // Public Element methods $('form').dirtyForm();
  $.fn.dirtyForms = function () {
    var core = $.DirtyForms;
    var thisForm = this;
    settings.deciding = false;
    settings.decidingEvent = false;
    settings.exitBound = false;

    dirtylog('Adding forms to watch');
    bindExit();

    return this.each(function (e) {
      dirtylog('Adding form ' + $(this).attr('id') + ' to forms to watch');
      $(this).addClass(core.listeningClass);
      $('input:text, input:password, input:checkbox, input:radio, textarea, select', this).change(function () {
        $(this).setDirty();
      });
    });
  }

  $.fn.setDirty = function () {
    dirtylog('setDirty called');
    return this.each(function (e) {
      $(this).addClass($.DirtyForms.dirtyClass).parents('form').addClass($.DirtyForms.dirtyClass);
    });
  }

  $.fn.resetDirty = function () {
    dirtylog('resetDirty called');
    return this.each(function (e) {
      $(this).removeClass($.DirtyForms.dirtyClass).parents('form').addClass($.DirtyForms.dirtyClass);
    });
  }

  $.fn.monitorDirty = function(ev) {
    dirtylog('monitorDirty called');
    return bindFn(ev);
  }

  // Returns true if any of the supplied elements are dirty
  $.fn.isDirty = function () {
    var isDirty = false;
    var node = this;
    this.each(function (e) {
      if ($(this).hasClass($.DirtyForms.dirtyClass)) {
        isDirty = true;
        return true;
      }
    });
    $.each($.DirtyForms.helpers, function (key, obj) {
      if ("isNodeDirty" in obj) {
        if (obj.isNodeDirty(node)) {
          isDirty = true;
          return true;
        }
      }
    });

    dirtylog('isDirty returned ' + isDirty);
    return isDirty;
  }

  // Private Properties and Methods
  var settings = $.extend({
    exitBound: false,
    formStash: false,
    dialogStash: false,
    deciding: false,
    decidingEvent: false,
    currentForm: false,
    hasFirebug: "console" in window && "firebug" in window.console,
    hasConsoleLog: "console" in window && "log" in window.console
  }, $.DirtyForms);

  dirtylog = function (msg) {
    if (!$.DirtyForms.debug) return;
    msg = "[DirtyForms] " + msg;
    settings.hasFirebug ?
			console.log(msg) :
			settings.hasConsoleLog ?
				window.console.log(msg) :
				alert(msg);
  }
  bindExit = function () {
    if (settings.exitBound) return;
    $('a').live('click', aBindFn);
    $('form').live('submit', formBindFn);
    // $(window).bind('beforeunload', beforeunloadBindFn);
    settings.exitBound = true;
  }

  aBindFn = function (ev) {
    bindFn(ev);
  }

  formBindFn = function (ev) {
    settings.currentForm = this;
    bindFn(ev);
  }

  beforeunloadBindFn = function (ev) {
    var result = bindFn(ev);

    if (result && settings.doubleunloadfix != true) {
      dirtylog('Before unload will be called, resetting');
      settings.deciding = false;
    }

    settings.doubleunloadfix = true;
    setTimeout(function () { settings.doubleunloadfix = false; }, 200);

    if (result === false) return null;
    return result;
  }

  bindFn = function (ev) {
    var $t;

    if (ev != undefined) {
      $t = $(ev.target);
      dirtylog('Entering: Leaving Event fired, type: ' + ev.type + ', element: ' + ev.target + ', class: ' + $t.attr('class') + ' and id: ' + ev.target.id);
    }

    if (ev != undefined && ev.type == 'beforeunload' && settings.doubleunloadfix) {
      dirtylog('Skip this unload, Firefox bug triggers the unload event multiple times');
      settings.doubleunloadfix = false;
      return false;
    }

    if (ev != undefined && ($t.hasClass(settings.ignoreClass) || ($t.closest("a.ui-datepicker-prev, a.ui-datepicker-next").length == 1) || ($t.closest("div#ui-datepicker-div").length == 1))) {
      dirtylog('Leaving: Element has ignore class');
      if (!ev.isDefaultPrevented()) {
        clearUnload();
      }
      return false;
    }

    if (settings.deciding) {
      dirtylog('Leaving: Already in the deciding process');
      return false;
    }

    if (ev != undefined && ev.isDefaultPrevented()) {
      dirtylog('Leaving: Event has been stopped elsewhere');
      return false;
    }

    if (!settings.isDirty()) {
      dirtylog('Leaving: Not dirty');
      if (ev != undefined && !ev.isDefaultPrevented()) {
        clearUnload();
      }
      return false;
    }

    if (ev != undefined && ev.type == 'submit' && $t.isDirty()) {
      dirtylog('Leaving: Form submitted is a dirty form');
      if (!ev.isDefaultPrevented()) {
        clearUnload();
      }
      return true;
    }

    settings.deciding = true;
    settings.decidingEvent = ev;
    dirtylog('Setting deciding active');

    if (settings.dialog !== false) {
      dirtylog('Saving dialog content');
      settings.dialogStash = settings.dialog.stash();
      dirtylog(settings.dialogStash);
    }

    // Callback for page access in current state
    $(document).trigger('defer.dirtyforms');

    if (ev != undefined && ev.type == 'beforeunload') {
      //clearUnload();
      dirtylog('Returning to beforeunload browser handler with: ' + settings.message);
      return settings.message;
    }
    if (!settings.dialog) return;

    if (ev != undefined) {
      ev.preventDefault();
      ev.stopImmediatePropagation();
    }

    if (ev != undefined && $t.is('form') && $t.parents(settings.dialog.selector).length > 0) {
      dirtylog('Stashing form');
      settings.formStash = $t.clone(true).hide();
    } else {
      settings.formStash = false;
    }

    dirtylog('Deferring to the dialog');
    settings.dialog.fire(settings.message, settings.title);
    settings.dialog.bind();
  }

  decidingCancel = function (ev) {
    ev.preventDefault();
    $(document).trigger('decidingcancelled.dirtyforms');
    if (settings.dialog !== false && settings.dialogStash !== false) {
      dirtylog('Re-firing the dialog with stashed content');
      settings.dialog.refire(settings.dialogStash.html(), ev);
    }
    $(document).trigger('decidingcancelledAfter.dirtyforms');
    settings.dialogStash = false;
    settings.deciding = settings.currentForm = settings.decidingEvent = false;
    try {
        $('#pendingChangesDialog').dialog('close');
    }catch ( error ) {}
  }

  decidingContinue = function (ev) {
    ev.preventDefault();
    settings.dialogStash = false;
    try {
        $('#pendingChangesDialog').dialog('close');
    } catch (error) { }
    $(document).trigger('decidingcontinued.dirtyforms');
    refire(settings.decidingEvent);
  }

  clearUnload = function () {
    // I'd like to just be able to unbind this but there seems
    // to be a bug in jQuery which doesn't unbind onbeforeunload
    dirtylog('Clearing the beforeunload event');
    $(window).unbind('beforeunload', beforeunloadBindFn);
    window.onbeforeunload = null;
  }

  refire = function (e) {
    $(document).trigger('beforeRefire.dirtyforms');
    var $t = $(e.target);

    switch (e.type) {
      case 'click':
        dirtylog("Re-firing click event");
        var event = new jQuery.Event('click');
        $t.trigger(event);
        if (!event.isDefaultPrevented()) {
          var loc = $t.attr('href');

          if (loc == undefined) {
            var $closestA = $t.closest('a');
            if ($closestA.length == 1) loc = $closestA.attr('href');
          }

          dirtylog('Sending location to ' + loc);
          location.href = loc;
          return;
        }
        break;
     case 'tabsbeforeactivate':
        $t.tabs("option", "active", e.data);      
        break;
      default:
        dirtylog("Re-firing " + e.type + " event on " + e.target);
        var target;
        if (settings.formStash) {
          dirtylog('Appending stashed form to body');
          target = settings.formStash;
          $('body').append(target);
        }
        else {
          target = $t;
          if (!target.is('form'))
            target = target.closest('form');
        }
        target.trigger(e.type);
        break;
    }
  }

})(jQuery);
