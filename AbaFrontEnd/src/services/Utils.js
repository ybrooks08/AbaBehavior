export default {
  getAddress (client) {
    const apt = client.apt ? `#${client.apt}` : ''
    const address = client.address ? client.address : 'NO ADDRESS'
    return `${address} ${apt}. ${client.city || ''}, ${client.state || ''} ${client.zipcode || ''}`
  },
}