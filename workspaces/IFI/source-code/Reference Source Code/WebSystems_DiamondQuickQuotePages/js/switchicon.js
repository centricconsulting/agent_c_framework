// -------------------------------------------------------------------
// Switch Content Script II (icon based)- By Dynamic Drive, available at: http://www.dynamicdrive.com
// April 8th, 07: Requires switchcontent.js!
// March 27th, 08': Added ability for certain headers to get its contents remotely from an external file via Ajax (2 variables within switchcontent.js to customize)
// -------------------------------------------------------------------

function switchicon(className, filtertag){
	switchcontent.call(this, className, filtertag) //inherit primary properties from switchcontent class
}

switchicon.prototype=new switchcontent //inherit methods from switchcontent class with its properties initialized already
switchicon.prototype.constructor=switchicon

switchicon.prototype.setStatus=null
switchicon.prototype.setColor=null

switchicon.prototype.setHeader=function(openHTML, closeHTML){ //PUBLIC
	this.openHTML=openHTML
	this.closeHTML=closeHTML
}

//PRIVATE: Contracts a content based on its corresponding header entered

switchicon.prototype.contractcontent=function(header){
	var innercontent=document.getElementById(header.id.replace("-title", "")) //Reference content for this header
	innercontent.style.display="none"
	header.innerHTML=this.closeHTML
	header=null
}


//PRIVATE: Expands a content based on its corresponding header entered

switchicon.prototype.expandcontent=function(header){
	var innercontent=document.getElementById(header.id.replace("-title", ""))
	if (header.ajaxstatus=="waiting"){//if this is an Ajax header AND remote content hasn't already been fetched
		switchcontent.connect(header.ajaxfile, header)
	}
	innercontent.style.display="block"
	header.innerHTML=this.openHTML
	header=null
}
