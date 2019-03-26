<template>
  <component :is="home"></component>
</template>

<script>


export default {
  computed: {
    user() {
      return this.$store.getters.user;
    },
    home() {
      return () => import(`@/views/homes/${this.user.template}`)
    }
  },
}
</script>
