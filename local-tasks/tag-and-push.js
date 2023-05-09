const gulp = requireModule("gulp-with-help"),
  gutil = requireModule("gulp-util"),
  gitPushTags = requireModule("git-push-tags"),
  gitPush = requireModule("git-push"),
  gitTagFromCsProj = requireModule("gulp-git-tag-from-csproj"),
  config = require("./config"),
  primaryPackage = config.packages[0],
  env = requireModule("env");

env.associate(["DRY_RUN"], ["tag-and-push", "push-tags"]);

gulp.task("tag-and-push", () => {
  return gulp.src(`**/${primaryPackage}/${primaryPackage}.csproj`)
    .pipe(gitTagFromCsProj({
      dryRun: env.resolveFlag("DRY_RUN")
    }));
});

gulp.task("push-tags", "Pushes tags and commits", () => {
  const dryRun = env.resolveFlag("DRY_RUN");
  return gitPushTags(dryRun)
    .then(() => gitPush(dryRun))
    .then(() => {
      if (!dryRun) {
        gutil.log(gutil.colors.green("-> all commits and tags pushed!"))
      }
    });
});
