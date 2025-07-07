<script>
    $.validator.addMethod("validemaildomain", function (value, element, params) {
        if (!value) return true;

    var domain = value.split('@')[1];
    if (!domain) return false;

    var allowed = params.split(',');
    return allowed.includes(domain.toLowerCase());
    });

    $.validator.unobtrusive.adapters.addSingleVal("validemaildomain", "domains");
</script>