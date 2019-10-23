define([
    "dojo/_base/declare",
    "dojo/aspect",
    "dojo/_base/lang",
    "dojo/topic",
    "epi/dependency",
    "epi/shell/_ContextMixin"
], function (
    declare,
    aspect,
    lang,
    topic,
    dependency,
    _ContextMixin
) {
    return declare([_ContextMixin], {
        currentPageId: null,

        initialize: function () {
            this.inherited(arguments);
            topic.subscribe("/epi/dnd/dropdata", lang.hitch(this, "_getDndData"));
            topic.subscribe("/dnd/drop", lang.hitch(this, "_onDndDrop"));
            topic.subscribe("/epi/shell/context/changed", lang.hitch(this, "_onContextChanged"));
        },

        _getDndData: function (item, acceptedTypes, internalDrop) {
            if (item[0] !== undefined) {
                var idProvider = item[0].data;
                var contentLink, contentId, provider;
                if (typeof (idProvider) === "string") {
                    if (!idProvider.includes("__")) {
                        contentLink = idProvider + "__" + "aprimo-dam";
                    } else {
                        contentLink = idProvider;
                    }
                } else if (typeof (idProvider) === "object") {
                    contentLink = idProvider.contentLink;
                }
                if (!contentLink.includes("__")) {
                    contentLink = contentLink + "__aprimo-dam";
                }

                if (contentLink !== undefined) {
                    require(["dojo/_base/xhr"],
                        function (xhr) {
                            xhr.get({
                                url: '/aprimo/aprimoapi/processdata/' + contentLink,
                                load: function (result) {
                                },
                                handle: function () {
                                    topic.publish("/epi/shell/context/request", {
                                        uri: currentPageId.requestedUri
                                    }, {
                                        sender: this,
                                        viewName: this.view,
                                        forceContextChange: true,
                                        forceReload: true
                                    });
                                }
                            });
                        });
                }
            }
        },

        _onDndDrop: function (source, item, dnd, target, event) {
            var newDiv = document.createElement("div");
            newDiv.setAttribute("class", "loading");
            newDiv.setAttribute("style", "position: absolute; top: 0; left: 0; background-color: rgba(20, 86, 241, .65); p color: #fff; padding: 3px 6px;");
            var newContent = document.createTextNode("Processing...");
            // add the text node to the newly created div
            newDiv.appendChild(newContent);
            target.node.appendChild(newDiv);
        },
        _onContextChanged: function (event) {
            currentPageId = event;
        }
    });
});