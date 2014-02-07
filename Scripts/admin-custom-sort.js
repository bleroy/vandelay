$(function() {
    var pinnedItems = $.map($("#pinnedItems").val().split(","),
            function(val) {
                var intval = parseInt(val, 10);
                return isNaN(intval) ? null : intval;
            }),
        pinnedItemsField = $("#pinnedItems"),
        setPinnedItems = function(pinnedIds) {
            pinnedItemsField.val(pinnedIds.join(","));
        },
        getItemId = function(el) {
            return parseInt(el.find(".summary").attr("itemid"), 10);
        },
        results = $("#results")
            .on("click", ".pin", function(e) {
                e.preventDefault();
                pin($(e.target).closest("li"));
            }),
        pager = $("#pager")
            .on("click", "a", function(e) {
                e.preventDefault();
                var page = parseInt($(e.target)
                    .html(), 10);
                loadQueryResults(currentQuery, page);
            }),
        pin = function(itemElement) {
            var newId = getItemId(itemElement);
            if ($.inArray(newId, pinnedItems) !== -1) return;
            pinned.find(".placeholder").remove();
            itemElement
                .clone()
                .removeClass("ui-draggable")
                .append(
                    $("<a class='remove-pinned' href='#'/>")
                    .html(window.customSortRemoveText)
                    .data("id", newId))
                .appendTo(pinned)
                .find(".pin").remove();
            pinnedItems.push(newId);
            setPinnedItems(pinnedItems);
        },
        pinned = $("#pinned-items")
            .droppable({
                hoverClass: "pinned-drag-hover",
                accept: ":not(.ui-sortable-helper)",
                activeClass: "pinned-drag-active"
            })
            .on("drop", function(e, ui) {
                pin(ui.draggable);
            })
            .on("click", ".remove-pinned", function(e) {
                e.preventDefault();
                var removeLink = $(e.target),
                    id = removeLink.data("id"),
                    index = $.inArray(id, pinnedItems);
                if (index !== -1) {
                    pinnedItems.splice(index, 1);
                    setPinnedItems(pinnedItems);
                    removeLink.closest("li").remove();
                }
            })
            .sortable({
                items: ">li:not(.placeholder)"
            })
            .on("sortupdate", function() {
                pinnedItems = pinned
                    .find(">li")
                    .map(function() {
                        return getItemId($(this));
                    })
                    .get();
                setPinnedItems(pinnedItems);
            })
            .find("li").each(function() {
                var item = $(this),
                    newId = getItemId(item);
                item.append(
                    $("<a class='remove-pinned' href='#'/>")
                    .html(window.customSortRemoveText)
                    .data("id", newId));
            })
            .end(),
        currentQuery,
        queryTotalCount,
        pageSize = 12,
        pageCount,
        buildPager = function(page) {
            var pageNumbers = new Array(pageCount);
            for (var i = pageCount; i > 0; i--) {
                pageNumbers[i] = i;
            }
            pager
                .empty()
                .append(
                    $.map(pageNumbers, function(n) {
                        if (n === page) {
                            return $("<li></li>")
                                .html(n);
                        } else {
                            return $("<li></li>")
                                .append(
                                    $("<a class='page-number' href='#'/>")
                                    .html(n));
                        }
                    })
                );
        },
        loadQueryResults = function(queryId, page) {
            queryDropDown.prop("disabled", true);
            $.post(window.customSortQueryUrlPattern, {
                    __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val(),
                    queryId: queryId,
                    skip: ((page || 1) - 1) * pageSize
                },
                function(data) {
                    currentQuery = queryId;
                    queryDropDown.prop("disabled", false);
                    queryTotalCount = $(data).data("total-count");
                    pageCount = Math.floor(queryTotalCount / pageSize) + 1;
                    var dataDomItems = $(data).find(">li");
                    results
                        .empty()
                        .append(dataDomItems)
                        .find("li").draggable({
                            helper: "clone"
                        })
                        .each(function() {
                            var item = $(this),
                                newId = getItemId(item);
                            item.append(
                                $("<a class='pin' href='#'/>")
                                .html(window.customSortPinText)
                                .data("id", newId));
                        });
                    if (pageCount > 1) {
                        buildPager(page || 1);
                    }
                });
        },
        queryDropDown = $("#Query")
            .on("change", function() {
                var queryId = $(this).val();
                if (queryId) {
                    loadQueryResults(queryId);
                }
            });
});