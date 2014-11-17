function AppViewModel() {
    var self = this;

    var maxPostId = 0;

    var maxPostIdForBatch = 0;

    self.days = ko.observableArray();

    self.posts = ko.observableArray();

    self.photos = ko.observableArray();

    self.getPage = function (id, pending) {
        if (id === undefined) {
            id = '';
        }

        if (pending === undefined) {
            pending = 300;
        }

        if (pending <= 0) {
            console.log("No pending posts");
            maxPostId = maxPostIdForBatch;
            console.log("Max post: " + maxPostId);
            setTimeout(function () { self.getPage(); }, 10000);
            return;
        }

        console.log("Pending " + pending + " posts");
        var apiUrl = 'dashboard/' + id;
        $.getJSON(apiUrl)
            .done(function(posts) {
                console.log("GET " + apiUrl);
                console.log(posts.length + " posts returned");

                posts = ko.utils.arrayFilter(posts, function (post) { return post.id > maxPostId; });


                console.log(posts.length + " posts newer than " + maxPostId);
                if (posts.length == 0) {
                    setTimeout(function () { self.getPage(); }, 10000);
                    return;
                }

                if (posts[0].id > maxPostIdForBatch) {
                    maxPostIdForBatch = posts[0].id;
                    console.log("Max post will be " + maxPostIdForBatch);
                }
                $.each(posts, function(_, post) {
                    completePost(post);
                    $.each(post.photos, function(photoSetIndex, photo) {
                        completePhoto(post, photo, photoSetIndex);
                        ko.utils.arrayForEach(photo.alt_sizes, function(photoInfo) {
                            completePhotoInfo(post, photo, photoInfo);
                        });

                        self.photos.push(photo);
                    });

                    self.posts.push(post);

                    self.posts.sort(function (left, right) { return left.timestamp == right.timestamp ? 0 : (left.timestamp > right.timestamp ? -1 : 1); });

                    
                });

                self.photos.sort(function (left, right) { return left.post.timestamp == right.post.timestamp ? 0 : (left.post.timestamp > right.post.timestamp ? -1 : 1); });
                self.getPage(posts[posts.length - 1].id, pending - posts.length);
            })
            .fail(function(a, b, c) {
            });
    };

    function completePost(post) {
        var date = moment.unix(post.timestamp);
        post.publish_date = date;
        if (post.tags === '' || post.tags === null || post.tags.length === 0) {
            post.tags = '(no tags)';
        }
    }

    function completePhoto(post, photo, photoSetIndex) {
        photo.full_size = photo.alt_sizes[0];
        photo.thumb_size_url = "gallery/image/" + post.blog_name + "/" + post.id + "/" + photoSetIndex + "/thumbnail/" + 250;
        photo.post = post;
        photo.publish_date = post.publish_date;
    }

    function completePhotoInfo(post, photo, photoInfo) {

    }

}